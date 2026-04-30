using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class StadiumRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Estadio>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT e.id_estadio, e.nombre, e.id_ciudad, e.capacidad, c.nombre ciudad, p.nombre pais
            FROM estadio e
            INNER JOIN ciudad c ON c.id_ciudad = e.id_ciudad
            INNER JOIN pais p ON p.id_pais = c.id_pais
            ORDER BY p.nombre, c.nombre, e.nombre
            """,
            reader => new Estadio
            {
                IdEstadio = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                IdCiudad = reader.GetInt32(2),
                Capacidad = reader.GetInt32(3),
                Ciudad = reader.GetString(4),
                Pais = reader.GetString(5)
            },
            cancellationToken: cancellationToken);

    public async Task SaveAsync(Estadio item, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(item, cancellationToken);

        if (item.IdEstadio == 0)
        {
            await ExecuteAsync(
                "INSERT INTO estadio (nombre, id_ciudad, capacidad) VALUES (:nombre, :id_ciudad, :capacidad)",
                new[] { Param("nombre", item.Nombre.Trim()), Param("id_ciudad", item.IdCiudad), Param("capacidad", item.Capacidad) },
                cancellationToken);

            return;
        }

        await ExecuteAsync(
            "UPDATE estadio SET nombre = :nombre, id_ciudad = :id_ciudad, capacidad = :capacidad WHERE id_estadio = :id_estadio",
            new[] { Param("nombre", item.Nombre.Trim()), Param("id_ciudad", item.IdCiudad), Param("capacidad", item.Capacidad), Param("id_estadio", item.IdEstadio) },
            cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM estadio WHERE id_estadio = :id", new[] { Param("id", id) }, cancellationToken);

    private async Task ValidateAsync(Estadio item, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(item.Nombre))
        {
            throw new InvalidOperationException("Debe ingresar el nombre del estadio.");
        }

        if (item.IdCiudad == 0)
        {
            throw new InvalidOperationException("Debe seleccionar una ciudad.");
        }

        if (item.Capacidad <= 0)
        {
            throw new InvalidOperationException("La capacidad del estadio debe ser mayor que cero.");
        }

        var exists = await QuerySingleAsync(
            "SELECT 1 FROM estadio WHERE UPPER(TRIM(nombre)) = UPPER(TRIM(:nombre)) AND id_ciudad = :id_ciudad AND id_estadio <> :id_estadio",
            reader => reader.GetInt32(0),
            new[]
            {
                Param("nombre", item.Nombre),
                Param("id_ciudad", item.IdCiudad),
                Param("id_estadio", item.IdEstadio)
            },
            cancellationToken);

        if (exists == 1)
        {
            throw new InvalidOperationException("Ya existe un estadio registrado con ese nombre para la ciudad seleccionada.");
        }
    }
}
