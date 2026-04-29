using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text.RegularExpressions;

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
        try
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
        catch (OracleException oe)
        {
            throw new InvalidOperationException(CleanOracleMessage(oe.Message), oe);
        }
    }

    protected async Task<T?> QuerySingleAsync<T>(string sql, Func<OracleDataReader, T> map, IEnumerable<OracleParameter>? parameters = null, CancellationToken cancellationToken = default)
    {
        var results = await QueryAsync(sql, map, parameters, cancellationToken);
        return results.FirstOrDefault();
    }

    protected async Task<int> ExecuteAsync(string sql, IEnumerable<OracleParameter>? parameters = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            await using var command = new OracleCommand(sql, connection)
            {
                BindByName = true
            };

            AddParameters(command, parameters);
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (OracleException oe)
        {
            throw new InvalidOperationException(CleanOracleMessage(oe.Message), oe);
        }
    }

    protected async Task<OracleCommand> CreateCommandAsync(string sql, IEnumerable<OracleParameter>? parameters = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            var command = new OracleCommand(sql, connection)
            {
                BindByName = true
            };

            AddParameters(command, parameters);
            return command;
        }
        catch (OracleException oe)
        {
            throw new InvalidOperationException(CleanOracleMessage(oe.Message), oe);
        }
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

    private static string CleanOracleMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return message ?? string.Empty;

        // Remove leading ORA-xxxxx: code
        var cleaned = Regex.Replace(message, "^ORA-\\d+:\\s*", "", RegexOptions.IgnoreCase);

        // Remove URLs (e.g., https://docs.oracle.com/...)
        cleaned = Regex.Replace(cleaned, "https?:\\/\\/[\\S]+", string.Empty, RegexOptions.IgnoreCase).Trim();

        // Normalize spacing and remove trailing punctuation from the URL removal
        cleaned = Regex.Replace(cleaned, "\\s{2,}", " ").Trim();

        return cleaned;
    }
    
}
