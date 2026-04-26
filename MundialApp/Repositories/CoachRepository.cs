using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class CoachRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<DirectorTecnico>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT d.id_dt, d.nombre, d.nacionalidad, d.id_equipo, e.nombre equipo
            FROM director_tecnico d
            INNER JOIN equipo e ON e.id_equipo = d.id_equipo
            ORDER BY e.nombre, d.nombre
            """,
            reader => new DirectorTecnico
            {
                IdDt = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Nacionalidad = reader.IsDBNull(2) ? null : reader.GetString(2),
                IdEquipo = reader.GetInt32(3),
                Equipo = reader.GetString(4)
            },
            cancellationToken: cancellationToken);

    public Task SaveAsync(DirectorTecnico item, CancellationToken cancellationToken = default)
        => item.IdDt == 0
            ? ExecuteAsync(
                "INSERT INTO director_tecnico (nombre, nacionalidad, id_equipo) VALUES (:nombre, :nacionalidad, :id_equipo)",
                new[] { Param("nombre", item.Nombre), Param("nacionalidad", item.Nacionalidad), Param("id_equipo", item.IdEquipo) },
                cancellationToken)
            : ExecuteAsync(
                "UPDATE director_tecnico SET nombre = :nombre, nacionalidad = :nacionalidad, id_equipo = :id_equipo WHERE id_dt = :id_dt",
                new[] { Param("nombre", item.Nombre), Param("nacionalidad", item.Nacionalidad), Param("id_equipo", item.IdEquipo), Param("id_dt", item.IdDt) },
                cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM director_tecnico WHERE id_dt = :id", new[] { Param("id", id) }, cancellationToken);
}
