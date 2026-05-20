using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class GolRepository(IOracleConnectionFactory connectionFactory)
    : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Gol>> GetByPartidoAsync(int idPartido, CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT g.id_gol, g.codigo, g.id_equipo, g.id_jugador, g.minuto,
                   j.nombre nombre_jugador, j.id_equipo,
                   e.nombre nombre_equipo
            FROM gol g
            INNER JOIN jugador j ON j.id_jugador = g.id_jugador AND j.id_equipo = g.id_equipo
            INNER JOIN equipo e ON e.id_equipo = g.id_equipo
            INNER JOIN participacion p ON p.codigo = g.codigo AND p.id_equipo = g.id_equipo
            WHERE p.id_partido = :id_partido
            ORDER BY g.minuto
            """,
            reader => new Gol
            {
                IdGol = reader.GetInt32(0),
                Codigo = reader.GetInt32(1),
                IdEquipo = reader.GetInt32(2),
                IdJugador = reader.GetInt32(3),
                Minuto = reader.GetInt32(4),
                Jugador = new Jugador
                {
                    IdJugador = reader.GetInt32(3),
                    Nombre = reader.GetString(5),
                    IdEquipo = reader.GetInt32(6)
                },
                Equipo = new Equipo
                {
                    IdEquipo = reader.GetInt32(2),
                    Nombre = reader.GetString(7)
                }
            },
            [Param("id_partido", idPartido)],
            cancellationToken);

    public Task<List<Gol>> GetByEquipoPartidoAsync(int idPartido, int idEquipo, CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT g.id_gol, g.codigo, g.id_equipo, g.id_jugador, g.minuto,
                   j.nombre nombre_jugador, j.id_equipo,
                   e.nombre nombre_equipo
            FROM gol g
            INNER JOIN jugador j ON j.id_jugador = g.id_jugador AND j.id_equipo = g.id_equipo
            INNER JOIN equipo e ON e.id_equipo = g.id_equipo
            INNER JOIN participacion p ON p.codigo = g.codigo AND p.id_equipo = g.id_equipo
            WHERE p.id_partido = :id_partido AND g.id_equipo = :id_equipo
            ORDER BY g.minuto
            """,
            reader => new Gol
            {
                IdGol = reader.GetInt32(0),
                Codigo = reader.GetInt32(1),
                IdEquipo = reader.GetInt32(2),
                IdJugador = reader.GetInt32(3),
                Minuto = reader.GetInt32(4),
                Jugador = new Jugador
                {
                    IdJugador = reader.GetInt32(3),
                    Nombre = reader.GetString(5),
                    IdEquipo = reader.GetInt32(6)
                },
                Equipo = new Equipo
                {
                    IdEquipo = reader.GetInt32(2),
                    Nombre = reader.GetString(7)
                }
            },
            [Param("id_partido", idPartido), Param("id_equipo", idEquipo)],
            cancellationToken);

    public Task SaveAsync(Gol item, CancellationToken cancellationToken = default)
        => item.IdGol == 0
            ? ExecuteAsync(
                """
                INSERT INTO gol (codigo, id_equipo, id_jugador, minuto)
                VALUES (:codigo, :id_equipo, :id_jugador, :minuto)
                """,
                [
                    Param("codigo", item.Codigo),
                    Param("id_equipo", item.IdEquipo),
                    Param("id_jugador", item.IdJugador),
                    Param("minuto", item.Minuto)
                ],
                cancellationToken)
            : ExecuteAsync(
                """
                UPDATE gol
                SET codigo = :codigo,
                    id_equipo = :id_equipo,
                    id_jugador = :id_jugador,
                    minuto = :minuto
                WHERE id_gol = :id_gol
                """,
                [
                    Param("codigo", item.Codigo),
                    Param("id_equipo", item.IdEquipo),
                    Param("id_jugador", item.IdJugador),
                    Param("minuto", item.Minuto),
                    Param("id_gol", item.IdGol)
                ],
                cancellationToken);

    public Task DeleteAsync(int idGol, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            "DELETE FROM gol WHERE id_gol = :id_gol",
            [Param("id_gol", idGol)],
            cancellationToken);

    public Task DeleteByPartidoAsync(int idPartido, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            """
            DELETE FROM gol
            WHERE codigo IN (
                SELECT codigo FROM participacion WHERE id_partido = :id_partido
            )
            """,
            [Param("id_partido", idPartido)],
            cancellationToken);
}
