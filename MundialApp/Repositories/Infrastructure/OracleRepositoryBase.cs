using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace MundialApp.Repositories.Infrastructure;

public abstract class OracleRepositoryBase(IOracleConnectionFactory connectionFactory)
{
    private readonly IOracleConnectionFactory _connectionFactory = connectionFactory;

    protected OracleParameter Param(string name, object? value, ParameterDirection direction = ParameterDirection.Input)
        => new(name, value ?? DBNull.Value)
        {
            Direction = direction
        };

    protected async Task<List<T>> QueryAsync<T>(string sql, Func<OracleDataReader, T> map, IEnumerable<OracleParameter>? parameters = null, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new OracleCommand(sql, connection)
        {
            BindByName = true
        };

        AddParameters(command, parameters);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var results = new List<T>();
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(map(reader));
        }

        return results;
    }

    protected async Task<T?> QuerySingleAsync<T>(string sql, Func<OracleDataReader, T> map, IEnumerable<OracleParameter>? parameters = null, CancellationToken cancellationToken = default)
    {
        var results = await QueryAsync(sql, map, parameters, cancellationToken);
        return results.FirstOrDefault();
    }

    protected async Task<int> ExecuteAsync(string sql, IEnumerable<OracleParameter>? parameters = null, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new OracleCommand(sql, connection)
        {
            BindByName = true
        };

        AddParameters(command, parameters);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    protected async Task<OracleCommand> CreateCommandAsync(string sql, IEnumerable<OracleParameter>? parameters = null, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new OracleCommand(sql, connection)
        {
            BindByName = true
        };

        AddParameters(command, parameters);
        return command;
    }

    private static void AddParameters(OracleCommand command, IEnumerable<OracleParameter>? parameters)
    {
        if (parameters is null)
        {
            return;
        }

        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }
    }
}
