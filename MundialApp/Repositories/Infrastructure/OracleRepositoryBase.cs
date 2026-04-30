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
            throw CreateFriendlyException(oe);
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
            throw CreateFriendlyException(oe);
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
            throw CreateFriendlyException(oe);
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

    protected static InvalidOperationException CreateFriendlyException(OracleException exception)
    {
        var cleanedMessage = CleanOracleMessage(exception.Message);
        var friendlyMessage = exception.Number switch
        {
            1 => BuildUniqueConstraintMessage(cleanedMessage),
            1400 => "Faltan datos obligatorios. Verifique los campos requeridos.",
            12899 => "Uno de los valores ingresados supera la longitud permitida.",
            1722 => "Uno de los valores numéricos ingresados no es válido.",
            2291 => "No se pudo guardar el registro porque uno de los datos relacionados no existe o no es válido.",
            2292 => "No se puede eliminar el registro porque está siendo utilizado en otras tablas.",
            _ => cleanedMessage
        };

        return new InvalidOperationException(friendlyMessage, exception);
    }

    private static string CleanOracleMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return message ?? string.Empty;

        var cleaned = Regex.Replace(message, "^ORA-\\d+:\\s*", "", RegexOptions.IgnoreCase);
        cleaned = Regex.Replace(cleaned, "https?:\\/\\/[\\S]+", string.Empty, RegexOptions.IgnoreCase).Trim();
        cleaned = Regex.Replace(cleaned, "\\s{2,}", " ").Trim();

        return cleaned;
    }

    private static string BuildUniqueConstraintMessage(string message)
    {
        var normalized = message.ToUpperInvariant();

        if (normalized.Contains("UQ_DT_EQUIPO"))
            return "Ya existe un director técnico asignado a ese equipo.";

        if (normalized.Contains("CORREO"))
            return "Ya existe un usuario registrado con ese correo.";

        if (normalized.Contains("CEDULA") || normalized.Contains("PK_USUARIO"))
            return "Ya existe un usuario registrado con esa cédula.";

        if (normalized.Contains("CONFEDERACION"))
            return "Ya existe una confederación registrada con ese nombre.";

        if (normalized.Contains("GRUPO"))
            return "Ya existe un grupo registrado con ese nombre.";

        if (normalized.Contains("PAIS"))
            return "Ya existe un país registrado con ese nombre.";

        if (normalized.Contains("CIUDAD"))
            return "Ya existe una ciudad registrada con ese nombre para el país seleccionado.";

        if (normalized.Contains("ESTADIO"))
            return "Ya existe un estadio registrado con ese nombre para la ciudad seleccionada.";

        if (normalized.Contains("EQUIPO"))
            return "Ya existe un equipo registrado con esos datos.";

        if (normalized.Contains("JUGADOR"))
            return "Ya existe un jugador registrado con esos datos.";

        return "Ya existe un registro con los mismos datos y no se puede repetir.";
    }
}
