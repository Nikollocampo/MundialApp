using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace MundialApp.Repositories;

public sealed class ParticipationRepository(IOracleConnectionFactory connectionFactory)
    : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Participacion>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT p.codigo, p.id_equipo, e.nombre AS nombre_equipo,
                   p.id_partido, p.condicion, p.sanciones,
                   pa.id_partido, pa.fecha,
                   el.nombre AS equipo_local, ev.nombre AS equipo_visitante
            FROM participacion p
            JOIN equipo e ON e.id_equipo = p.id_equipo
            JOIN partido pa ON pa.id_partido = p.id_partido
            LEFT JOIN participacion pl ON pl.id_partido = pa.id_partido AND pl.condicion = 'LOCAL'
            LEFT JOIN equipo el ON el.id_equipo = pl.id_equipo
            LEFT JOIN participacion pv ON pv.id_partido = pa.id_partido AND pv.condicion = 'VISITANTE'
            LEFT JOIN equipo ev ON ev.id_equipo = pv.id_equipo
            ORDER BY pa.fecha DESC
            """,
            reader => new Participacion
            {
                Codigo = reader.GetInt32(0),
                IdEquipo = reader.GetInt32(1),
                Equipo = new Equipo { IdEquipo = reader.GetInt32(1), Nombre = reader.GetString(2) },
                IdPartido = reader.GetInt32(3),
                Condicion = reader.GetString(4),
                Sanciones = reader.IsDBNull(5) ? null : reader.GetString(5),
                Partido = new Partido
                {
                    IdPartido = reader.GetInt32(6),
                    Fecha = reader.GetDateTime(7),
                    EquipoLocal = reader.IsDBNull(8) ? "Por definir" : reader.GetString(8),
                    EquipoVisitante = reader.IsDBNull(9) ? "Por definir" : reader.GetString(9)
                }
            },
            cancellationToken: cancellationToken);

    public Task<List<Participacion>> GetByPartidoAsync(int idPartido, CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT p.codigo, p.id_equipo, e.nombre AS nombre_equipo,
                   p.id_partido, p.condicion, p.sanciones,
                   pa.id_partido, pa.fecha,
                   el.nombre AS equipo_local, ev.nombre AS equipo_visitante
            FROM participacion p
            JOIN equipo e ON e.id_equipo = p.id_equipo
            JOIN partido pa ON pa.id_partido = p.id_partido
            LEFT JOIN participacion pl ON pl.id_partido = pa.id_partido AND pl.condicion = 'LOCAL'
            LEFT JOIN equipo el ON el.id_equipo = pl.id_equipo
            LEFT JOIN participacion pv ON pv.id_partido = pa.id_partido AND pv.condicion = 'VISITANTE'
            LEFT JOIN equipo ev ON ev.id_equipo = pv.id_equipo
            WHERE p.id_partido = :id_partido
            """,
            reader => new Participacion
            {
                Codigo = reader.GetInt32(0),
                IdEquipo = reader.GetInt32(1),
                Equipo = new Equipo { IdEquipo = reader.GetInt32(1), Nombre = reader.GetString(2) },
                IdPartido = reader.GetInt32(3),
                Condicion = reader.GetString(4),
                Sanciones = reader.IsDBNull(5) ? null : reader.GetString(5),
                Partido = new Partido
                {
                    IdPartido = reader.GetInt32(6),
                    Fecha = reader.GetDateTime(7),
                    EquipoLocal = reader.IsDBNull(8) ? "Por definir" : reader.GetString(8),
                    EquipoVisitante = reader.IsDBNull(9) ? "Por definir" : reader.GetString(9)
                }
            },
            parameters: [Param("id_partido", idPartido)],
            cancellationToken: cancellationToken);

    public Task SaveAsync(Participacion item, CancellationToken cancellationToken = default)
        => item.Codigo == 0
            ? ExecuteAsync(
                """
                INSERT INTO participacion (id_equipo, id_partido, condicion, sanciones)
                VALUES (:id_equipo, :id_partido, :condicion, :sanciones)
                """,
                [
                    Param("id_equipo",  item.IdEquipo),
                    Param("id_partido", item.IdPartido),
                    Param("condicion",  item.Condicion),
                    Param("sanciones",  (object?)item.Sanciones ?? DBNull.Value)
                ],
                cancellationToken)
            : ExecuteAsync(
                """
                UPDATE participacion
                SET sanciones = :sanciones
                WHERE codigo = :codigo
                """,
                [
                    Param("sanciones", (object?)item.Sanciones ?? DBNull.Value),
                    Param("codigo",    item.Codigo)
                ],
                cancellationToken);

    public Task DeleteAsync(int codigo, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            "DELETE FROM participacion WHERE codigo = :codigo",
            [Param("codigo", codigo)],
            cancellationToken);
}