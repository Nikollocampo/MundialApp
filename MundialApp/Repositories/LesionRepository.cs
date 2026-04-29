using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class LesionRepository(IOracleConnectionFactory connectionFactory)
    : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Lesion>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT l.id_lesion, l.id_jugador, j.nombre AS nombre_jugador,
                   l.fecha_inicio, l.descripcion
            FROM lesion l
            JOIN jugador j ON j.id_jugador = l.id_jugador
            ORDER BY l.fecha_inicio DESC
            """,
            reader => new Lesion
            {
                IdLesion = reader.GetInt32(0),
                IdJugador = reader.GetInt32(1),
                Jugador = new Jugador { IdJugador = reader.GetInt32(1), Nombre = reader.GetString(2) },
                FechaInicio = reader.GetDateTime(3),
                Descripcion = reader.GetString(4)
            },
            cancellationToken: cancellationToken);

    public Task<List<Lesion>> GetByJugadorAsync(int idJugador, CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT id_lesion, id_jugador, fecha_inicio, descripcion
            FROM lesion
            WHERE id_jugador = :id_jugador
            ORDER BY fecha_inicio DESC
            """,
            reader => new Lesion
            {
                IdLesion = reader.GetInt32(0),
                IdJugador = reader.GetInt32(1),
                FechaInicio = reader.GetDateTime(2),
                Descripcion = reader.GetString(3)
            },
            parameters: [Param("id_jugador", idJugador)],
            cancellationToken: cancellationToken);

    public Task SaveAsync(Lesion item, CancellationToken cancellationToken = default)
        => item.IdLesion == 0
            ? ExecuteAsync(
                """
                INSERT INTO lesion (id_jugador, fecha_inicio, descripcion)
                VALUES (:id_jugador, :fecha_inicio, :descripcion)
                """,
                [
                    Param("id_jugador",   item.IdJugador),
                    Param("fecha_inicio", item.FechaInicio),
                    Param("descripcion",  item.Descripcion)
                ],
                cancellationToken)
            : ExecuteAsync(
                """
                UPDATE lesion
                SET fecha_inicio = :fecha_inicio,
                    descripcion  = :descripcion
                WHERE id_lesion = :id_lesion
                """,
                [
                    Param("fecha_inicio", item.FechaInicio),
                    Param("descripcion",  item.Descripcion),
                    Param("id_lesion",    item.IdLesion)
                ],
                cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            "DELETE FROM lesion WHERE id_lesion = :id",
            [Param("id", id)],
            cancellationToken);
}

