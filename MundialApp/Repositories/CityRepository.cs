using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class CityRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Ciudad>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT c.id_ciudad, c.nombre, c.id_pais, p.nombre pais
            FROM ciudad c
            INNER JOIN pais p ON p.id_pais = c.id_pais
            ORDER BY p.nombre, c.nombre
            """,
            reader => new Ciudad
            {
                IdCiudad = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                IdPais = reader.GetInt32(2),
                Pais = reader.GetString(3)
            },
            cancellationToken: cancellationToken);

    public async Task SaveAsync(Ciudad item, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(item, cancellationToken);

        if (item.IdCiudad == 0)
        {
            await ExecuteAsync(
                "INSERT INTO ciudad (nombre, id_pais) VALUES (:nombre, :id_pais)",
                new[] { Param("nombre", item.Nombre.Trim()), Param("id_pais", item.IdPais) },
                cancellationToken);

            return;
        }

        await ExecuteAsync(
            "UPDATE ciudad SET nombre = :nombre, id_pais = :id_pais WHERE id_ciudad = :id_ciudad",
            new[] { Param("nombre", item.Nombre.Trim()), Param("id_pais", item.IdPais), Param("id_ciudad", item.IdCiudad) },
            cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM ciudad WHERE id_ciudad = :id", new[] { Param("id", id) }, cancellationToken);

    private async Task ValidateAsync(Ciudad item, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(item.Nombre))
        {
            throw new InvalidOperationException("Debe ingresar el nombre de la ciudad.");
        }

        if (item.IdPais == 0)
        {
            throw new InvalidOperationException("Debe seleccionar un país.");
        }

        var exists = await QuerySingleAsync(
            "SELECT 1 FROM ciudad WHERE UPPER(TRIM(nombre)) = UPPER(TRIM(:nombre)) AND id_pais = :id_pais AND id_ciudad <> :id_ciudad",
            reader => reader.GetInt32(0),
            new[]
            {
                Param("nombre", item.Nombre),
                Param("id_pais", item.IdPais),
                Param("id_ciudad", item.IdCiudad)
            },
            cancellationToken);

        if (exists == 1)
        {
            throw new InvalidOperationException("Ya existe una ciudad registrada con ese nombre para el país seleccionado.");
        }
    }
}
