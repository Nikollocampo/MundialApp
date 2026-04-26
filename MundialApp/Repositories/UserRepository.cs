using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class UserRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Usuario>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT cedula, nombre, correo, contrasena, telefono, rol FROM usuario ORDER BY nombre",
            reader => new Usuario
            {
                Cedula = reader.GetString(0),
                Nombre = reader.GetString(1),
                Correo = reader.GetString(2),
                Contrasena = reader.GetString(3),
                Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                Rol = reader.GetString(5)
            },
            cancellationToken: cancellationToken);

    public Task SaveAsync(Usuario usuario, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            """
            MERGE INTO usuario target
            USING (SELECT :cedula cedula FROM dual) source
            ON (target.cedula = source.cedula)
            WHEN MATCHED THEN UPDATE SET
                nombre = :nombre,
                correo = :correo,
                contrasena = :contrasena,
                telefono = :telefono,
                rol = :rol
            WHEN NOT MATCHED THEN INSERT (cedula, nombre, correo, contrasena, telefono, rol)
                VALUES (:cedula, :nombre, :correo, :contrasena, :telefono, :rol)
            """,
            new[]
            {
                Param("cedula", usuario.Cedula),
                Param("nombre", usuario.Nombre),
                Param("correo", usuario.Correo),
                Param("contrasena", usuario.Contrasena),
                Param("telefono", usuario.Telefono),
                Param("rol", usuario.Rol)
            },
            cancellationToken);

    public Task DeleteAsync(string cedula, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM usuario WHERE cedula = :cedula", new[] { Param("cedula", cedula) }, cancellationToken);
}
