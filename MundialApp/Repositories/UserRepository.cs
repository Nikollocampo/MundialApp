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

    public async Task SaveAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(usuario, cancellationToken);

        usuario.Cedula = usuario.Cedula.Trim();
        usuario.Nombre = usuario.Nombre.Trim();
        usuario.Correo = usuario.Correo.Trim();
        usuario.Contrasena = usuario.Contrasena.Trim();
        usuario.Telefono = string.IsNullOrWhiteSpace(usuario.Telefono) ? null : usuario.Telefono.Trim();
        usuario.Rol = usuario.Rol.Trim().ToUpperInvariant();

        await ExecuteAsync(
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
    }

    public Task DeleteAsync(string cedula, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM usuario WHERE cedula = :cedula", new[] { Param("cedula", cedula) }, cancellationToken);

    private async Task ValidateAsync(Usuario usuario, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(usuario.Cedula))
        {
            throw new InvalidOperationException("Debe ingresar la cédula del usuario.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Nombre))
        {
            throw new InvalidOperationException("Debe ingresar el nombre del usuario.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Correo))
        {
            throw new InvalidOperationException("Debe ingresar el correo del usuario.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Contrasena))
        {
            throw new InvalidOperationException("Debe ingresar la contraseña del usuario.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Rol))
        {
            throw new InvalidOperationException("Debe seleccionar un rol.");
        }

        var duplicatedEmail = await QuerySingleAsync(
            "SELECT 1 FROM usuario WHERE UPPER(TRIM(correo)) = UPPER(TRIM(:correo)) AND cedula <> :cedula",
            reader => reader.GetInt32(0),
            new[]
            {
                Param("correo", usuario.Correo),
                Param("cedula", usuario.Cedula)
            },
            cancellationToken);

        if (duplicatedEmail == 1)
        {
            throw new InvalidOperationException("Ya existe un usuario registrado con ese correo.");
        }

        var adminCount = await QuerySingleAsync(
            "SELECT COUNT(1) FROM usuario WHERE UPPER(rol) = 'ADMINISTRADOR' AND cedula <> :cedula",
            reader => reader.GetInt32(0),
            new[] { Param("cedula", usuario.Cedula) },
            cancellationToken);

        if (string.Equals(usuario.Rol, "ADMINISTRADOR", StringComparison.OrdinalIgnoreCase) && adminCount > 0)
        {
            throw new InvalidOperationException("Actualmente el sistema no tiene un campo de estado para inactivar automáticamente al administrador anterior. Es necesario agregar esa columna en la tabla USUARIO para aplicar esa regla correctamente.");
        }
    }
}
