using MundialApp.Models.Dto;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class QueryRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<JugadorCostosoPorConfederacionDto>> GetMostExpensivePlayerByConfederationAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT confederacion, jugador, equipo, costo
            FROM (
                SELECT c.nombre confederacion,
                       j.nombre jugador,
                       e.nombre equipo,
                       j.costo,
                       ROW_NUMBER() OVER (PARTITION BY c.id_confederacion ORDER BY j.costo DESC, j.nombre) rn
                FROM jugador j
                INNER JOIN equipo e ON e.id_equipo = j.id_equipo
                INNER JOIN confederacion c ON c.id_confederacion = e.id_confederacion
            )
            WHERE rn = 1
            ORDER BY confederacion
            """,
            reader => new JugadorCostosoPorConfederacionDto
            {
                Confederacion = reader.GetString(0),
                Jugador = reader.GetString(1),
                Equipo = reader.GetString(2),
                Costo = reader.GetDecimal(3)
            },
            cancellationToken: cancellationToken);

    public Task<List<PartidoPorEstadioDto>> GetMatchesByStadiumAsync(int idEstadio, CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT p.id_partido,
                   es.nombre estadio,
                   c.nombre ciudad,
                   pa.nombre pais,
                   p.fecha,
                   el.nombre equipo_local,
                   ev.nombre equipo_visitante
            FROM partido p
            INNER JOIN estadio es ON es.id_estadio = p.id_estadio
            INNER JOIN ciudad c ON c.id_ciudad = es.id_ciudad
            INNER JOIN pais pa ON pa.id_pais = c.id_pais
            INNER JOIN participacion pl ON pl.id_partido = p.id_partido AND pl.condicion = 'LOCAL'
            INNER JOIN participacion pv ON pv.id_partido = p.id_partido AND pv.condicion = 'VISITANTE'
            INNER JOIN equipo el ON el.id_equipo = pl.id_equipo
            INNER JOIN equipo ev ON ev.id_equipo = pv.id_equipo
            WHERE p.id_estadio = :id_estadio
            ORDER BY p.fecha
            """,
            reader => new PartidoPorEstadioDto
            {
                IdPartido = reader.GetInt32(0),
                Estadio = reader.GetString(1),
                Ciudad = reader.GetString(2),
                Pais = reader.GetString(3),
                Fecha = reader.GetDateTime(4),
                EquipoLocal = reader.GetString(5),
                EquipoVisitante = reader.GetString(6)
            },
            new[] { Param("id_estadio", idEstadio) },
            cancellationToken);

    public Task<List<EquipoCostosoPorPaisAnfitrionDto>> GetMostExpensiveTeamByHostCountryAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT pais_anfitrion, equipo, valor_total
            FROM (
                SELECT pa.nombre pais_anfitrion,
                       e.nombre equipo,
                       SUM(j.costo) valor_total,
                       ROW_NUMBER() OVER (PARTITION BY pa.id_pais ORDER BY SUM(j.costo) DESC, e.nombre) rn
                FROM partido p
                INNER JOIN estadio es ON es.id_estadio = p.id_estadio
                INNER JOIN ciudad c ON c.id_ciudad = es.id_ciudad
                INNER JOIN pais pa ON pa.id_pais = c.id_pais
                INNER JOIN participacion part ON part.id_partido = p.id_partido
                INNER JOIN equipo e ON e.id_equipo = part.id_equipo
                INNER JOIN jugador j ON j.id_equipo = e.id_equipo
                WHERE pa.es_anfitrion = 1
                GROUP BY pa.id_pais, pa.nombre, e.nombre
            )
            WHERE rn = 1
            ORDER BY pais_anfitrion
            """,
            reader => new EquipoCostosoPorPaisAnfitrionDto
            {
                PaisAnfitrion = reader.GetString(0),
                Equipo = reader.GetString(1),
                ValorTotal = reader.GetDecimal(2)
            },
            cancellationToken: cancellationToken);

    public Task<List<CantidadJugadoresJovenesDto>> GetYoungPlayersCountByTeamAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT e.nombre equipo,
                   COUNT(*) cantidad_menores
            FROM jugador j
            INNER JOIN equipo e ON e.id_equipo = j.id_equipo
            WHERE TRUNC((SYSDATE - j.fecha_nacimiento) / 365.25) < 21
            GROUP BY e.nombre
            ORDER BY e.nombre
            """,
            reader => new CantidadJugadoresJovenesDto
            {
                Equipo = reader.GetString(0),
                CantidadMenoresDe21 = reader.GetInt32(1)
            },
            cancellationToken: cancellationToken);
    }
