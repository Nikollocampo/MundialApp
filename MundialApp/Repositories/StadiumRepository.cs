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

    public Task SaveAsync(Estadio item, CancellationToken cancellationToken = default)
        => item.IdEstadio == 0
            ? ExecuteAsync(
                "INSERT INTO estadio (nombre, id_ciudad, capacidad) VALUES (:nombre, :id_ciudad, :capacidad)",
                new[] { Param("nombre", item.Nombre), Param("id_ciudad", item.IdCiudad), Param("capacidad", item.Capacidad) },
                cancellationToken)
            : ExecuteAsync(
                "UPDATE estadio SET nombre = :nombre, id_ciudad = :id_ciudad, capacidad = :capacidad WHERE id_estadio = :id_estadio",
                new[] { Param("nombre", item.Nombre), Param("id_ciudad", item.IdCiudad), Param("capacidad", item.Capacidad), Param("id_estadio", item.IdEstadio) },
                cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM estadio WHERE id_estadio = :id", new[] { Param("id", id) }, cancellationToken);
}
