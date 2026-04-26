using MundialApp.Models.Dto;
using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class DashboardRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public async Task<DashboardSummary> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var summary = new DashboardSummary
        {
            TotalEquipos = await CountAsync("SELECT COUNT(*) FROM equipo", cancellationToken),
            TotalJugadores = await CountAsync("SELECT COUNT(*) FROM jugador", cancellationToken),
            TotalPartidos = await CountAsync("SELECT COUNT(*) FROM partido", cancellationToken),
            TotalEstadios = await CountAsync("SELECT COUNT(*) FROM estadio", cancellationToken),
            TotalUsuarios = await CountAsync("SELECT COUNT(*) FROM usuario", cancellationToken),
            ProximosPartidos = await GetUpcomingMatchesAsync(cancellationToken),
            PaisesAnfitriones = await QueryAsync(
                "SELECT nombre FROM pais WHERE es_anfitrion = 1 ORDER BY nombre",
                reader => reader.GetString(0),
                cancellationToken: cancellationToken)
        };

        return summary;
    }

    private async Task<int> CountAsync(string sql, CancellationToken cancellationToken)
    {
        var values = await QueryAsync(sql, reader => reader.GetInt32(0), cancellationToken: cancellationToken);
        return values.FirstOrDefault();
    }

    private Task<List<Partido>> GetUpcomingMatchesAsync(CancellationToken cancellationToken)
        => QueryAsync(
            """
            SELECT *
            FROM (
                SELECT p.id_partido,
                       p.id_estadio,
                       p.fecha,
                       el.id_equipo id_local,
                       el.nombre equipo_local,
                       ev.id_equipo id_visitante,
                       ev.nombre equipo_visitante,
                       es.nombre estadio,
                       c.nombre ciudad,
                       pa.nombre pais
                FROM partido p
                INNER JOIN estadio es ON es.id_estadio = p.id_estadio
                INNER JOIN ciudad c ON c.id_ciudad = es.id_ciudad
                INNER JOIN pais pa ON pa.id_pais = c.id_pais
                INNER JOIN participacion pl ON pl.id_partido = p.id_partido AND pl.condicion = 'LOCAL'
                INNER JOIN participacion pv ON pv.id_partido = p.id_partido AND pv.condicion = 'VISITANTE'
                INNER JOIN equipo el ON el.id_equipo = pl.id_equipo
                INNER JOIN equipo ev ON ev.id_equipo = pv.id_equipo
                ORDER BY p.fecha
            )
            WHERE ROWNUM <= 5
            """,
            reader => new Partido
            {
                IdPartido = reader.GetInt32(0),
                IdEstadio = reader.GetInt32(1),
                Fecha = reader.GetDateTime(2),
                IdLocal = reader.GetInt32(3),
                EquipoLocal = reader.GetString(4),
                IdVisitante = reader.GetInt32(5),
                EquipoVisitante = reader.GetString(6),
                Estadio = reader.GetString(7),
                Ciudad = reader.GetString(8),
                Pais = reader.GetString(9)
            },
            cancellationToken: cancellationToken);
}
