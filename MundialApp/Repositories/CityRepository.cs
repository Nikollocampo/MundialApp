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

    public Task SaveAsync(Ciudad item, CancellationToken cancellationToken = default)
        => item.IdCiudad == 0
            ? ExecuteAsync(
                "INSERT INTO ciudad (nombre, id_pais) VALUES (:nombre, :id_pais)",
                new[] { Param("nombre", item.Nombre), Param("id_pais", item.IdPais) },
                cancellationToken)
            : ExecuteAsync(
                "UPDATE ciudad SET nombre = :nombre, id_pais = :id_pais WHERE id_ciudad = :id_ciudad",
                new[] { Param("nombre", item.Nombre), Param("id_pais", item.IdPais), Param("id_ciudad", item.IdCiudad) },
                cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM ciudad WHERE id_ciudad = :id", new[] { Param("id", id) }, cancellationToken);
}
