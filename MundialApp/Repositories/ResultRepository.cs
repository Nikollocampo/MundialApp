using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class ResultRepository(IOracleConnectionFactory connectionFactory)
                    
    : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Resultado>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT r.id_resultado, r.id_partido, r.goles_local,
                   r.goles_visitante, r.ganador,
                   p.id_partido, p.fecha,
                   el.nombre AS equipo_local, ev.nombre AS equipo_visitante,
                   eg.id_equipo, eg.nombre AS equipo_ganador_nombre
            FROM resultado r
            JOIN partido p ON p.id_partido = r.id_partido
            LEFT JOIN participacion pl ON pl.id_partido = p.id_partido AND pl.condicion = 'LOCAL'
            LEFT JOIN equipo el ON el.id_equipo = pl.id_equipo
            LEFT JOIN participacion pv ON pv.id_partido = p.id_partido AND pv.condicion = 'VISITANTE'
            LEFT JOIN equipo ev ON ev.id_equipo = pv.id_equipo
            LEFT JOIN equipo eg ON eg.id_equipo = r.ganador
            ORDER BY p.fecha DESC
            """,
            reader => new Resultado
            {
                IdResultado = reader.GetInt32(0),
                IdPartido = reader.GetInt32(1),
                GolesLocal = reader.GetInt32(2),
                GolesVisitante = reader.GetInt32(3),
                Ganador = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                Partido = new Partido
                {
                    IdPartido = reader.GetInt32(5),
                    Fecha = reader.GetDateTime(6),
                    EquipoLocal = reader.IsDBNull(7) ? "Por definir" : reader.GetString(7),
                    EquipoVisitante = reader.IsDBNull(8) ? "Por definir" : reader.GetString(8)
                },
                EquipoGanador = reader.IsDBNull(9) ? null : new Equipo
                {
                    IdEquipo = reader.GetInt32(9),
                    Nombre = reader.GetString(10)
                }
            },
            cancellationToken: cancellationToken);

    public Task<Resultado?> GetByPartidoAsync(int idPartido, CancellationToken cancellationToken = default)
        => QuerySingleAsync(
            """
            SELECT id_resultado, id_partido, goles_local, goles_visitante, ganador
            FROM resultado
            WHERE id_partido = :id_partido
            """,
            reader => new Resultado
            {
                IdResultado = reader.GetInt32(0),
                IdPartido = reader.GetInt32(1),
                GolesLocal = reader.GetInt32(2),
                GolesVisitante = reader.GetInt32(3),
                Ganador = reader.IsDBNull(4) ? null : reader.GetInt32(4)
            },
            parameters: [Param("id_partido", idPartido)],
            cancellationToken: cancellationToken);

    public Task SaveAsync(Resultado item, CancellationToken cancellationToken = default)
        => item.IdResultado == 0
            ? ExecuteAsync(
                """
                INSERT INTO resultado (id_partido, goles_local, goles_visitante, ganador)
                VALUES (:id_partido, :goles_local, :goles_visitante, :ganador)
                """,
                [
                    Param("id_partido",      item.IdPartido),
                    Param("goles_local",     item.GolesLocal),
                    Param("goles_visitante", item.GolesVisitante),
                    Param("ganador",         (object?)item.Ganador ?? DBNull.Value)
                ],
                cancellationToken)
            : ExecuteAsync(
                """
                UPDATE resultado
                SET goles_local     = :goles_local,
                    goles_visitante = :goles_visitante,
                    ganador         = :ganador
                WHERE id_resultado = :id_resultado
                """,
                [
                    Param("goles_local",     item.GolesLocal),
                    Param("goles_visitante", item.GolesVisitante),
                    Param("ganador",         (object?)item.Ganador ?? DBNull.Value),
                    Param("id_resultado",    item.IdResultado)
                ],
                cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            "DELETE FROM resultado WHERE id_resultado = :id",
            [Param("id", id)],
            cancellationToken);
}