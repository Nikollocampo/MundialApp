using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace MundialApp.Repositories;

public sealed class AuthRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<Usuario?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
        => QuerySingleAsync(
            """
            SELECT cedula, nombre, correo, contrasena, telefono, rol
            FROM usuario
            WHERE cedula = :identificador OR UPPER(correo) = UPPER(:identificador)
            """,
            reader => new Usuario
            {
                Cedula = reader.GetString(0),
                Nombre = reader.GetString(1),
                Correo = reader.GetString(2),
                Contrasena = reader.GetString(3),
                Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                Rol = reader.GetString(5)
            },
            new[] { Param("identificador", identifier) },
            cancellationToken);

    public async Task<int> RegisterLoginAsync(string cedula, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO bitacora (id_usuario, fecha_entrada, accion)
            VALUES (:id_usuario, SYSTIMESTAMP, 'INGRESO')
            RETURNING id_bitacora INTO :id_bitacora
            """;

        var output = new OracleParameter("id_bitacora", OracleDbType.Int32)
        {
            Direction = ParameterDirection.Output
        };

        await using var command = await CreateCommandAsync(sql, new[]
        {
            Param("id_usuario", cedula),
            output
        }, cancellationToken);

        await command.ExecuteNonQueryAsync(cancellationToken);
        var value = (OracleDecimal)output.Value;
        await command.Connection!.DisposeAsync();
        return value.ToInt32();
    }

    public Task RegisterLogoutAsync(int bitacoraId, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            """
            UPDATE bitacora
            SET fecha_salida = SYSTIMESTAMP,
                accion = 'SALIDA'
            WHERE id_bitacora = :id_bitacora
            """,
            new[] { Param("id_bitacora", bitacoraId) },
            cancellationToken);
}
