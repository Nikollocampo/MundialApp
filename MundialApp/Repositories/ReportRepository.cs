using MundialApp.Models.Dto;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class ReportRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<BitacoraReporteItem>> GetBitacoraByDateRangeAsync(DateTime desde, DateTime hasta, CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT b.id_usuario, u.nombre, b.fecha_entrada, b.fecha_salida, b.accion
            FROM bitacora b
            INNER JOIN usuario u ON u.cedula = b.id_usuario
            WHERE b.fecha_entrada BETWEEN :desde AND :hasta
               OR NVL(b.fecha_salida, b.fecha_entrada) BETWEEN :desde AND :hasta
            ORDER BY b.fecha_entrada
            """,
            reader => new BitacoraReporteItem
            {
                Usuario = reader.GetString(0),
                Nombre = reader.GetString(1),
                FechaEntrada = reader.GetDateTime(2),
                FechaSalida = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                Accion = reader.IsDBNull(4) ? null : reader.GetString(4)
            },
            new[] { Param("desde", desde), Param("hasta", hasta) },
            cancellationToken);

    public Task<List<JugadorReporteItem>> GetPlayersByFiltersAsync(decimal? pesoMin, decimal? pesoMax, decimal? alturaMin, decimal? alturaMax, int? idEquipo, CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT j.nombre, j.peso, j.altura, e.nombre equipo
            FROM jugador j
            INNER JOIN equipo e ON e.id_equipo = j.id_equipo
            WHERE (:peso_min IS NULL OR j.peso >= :peso_min)
              AND (:peso_max IS NULL OR j.peso <= :peso_max)
              AND (:altura_min IS NULL OR j.altura >= :altura_min)
              AND (:altura_max IS NULL OR j.altura <= :altura_max)
              AND (:id_equipo IS NULL OR j.id_equipo = :id_equipo)
            ORDER BY e.nombre, j.nombre
            """,
            reader => new JugadorReporteItem
            {
                Jugador = reader.GetString(0),
                Peso = reader.IsDBNull(1) ? null : reader.GetDecimal(1),
                Altura = reader.IsDBNull(2) ? null : reader.GetDecimal(2),
                Equipo = reader.GetString(3)
            },
            new[]
            {
                Param("peso_min", pesoMin),
                Param("peso_max", pesoMax),
                Param("altura_min", alturaMin),
                Param("altura_max", alturaMax),
                Param("id_equipo", idEquipo)
            },
            cancellationToken);

    public Task<List<ValorEquipoConfederacionReporteItem>> GetTeamValueByConfederationAsync(int idConfederacion, CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT c.nombre confederacion, e.nombre equipo, SUM(j.costo) valor_total
            FROM equipo e
            INNER JOIN confederacion c ON c.id_confederacion = e.id_confederacion
            INNER JOIN jugador j ON j.id_equipo = e.id_equipo
            WHERE e.id_confederacion = :id_confederacion
            GROUP BY c.nombre, e.nombre
            ORDER BY e.nombre
            """,
            reader => new ValorEquipoConfederacionReporteItem
            {
                Confederacion = reader.GetString(0),
                Equipo = reader.GetString(1),
                ValorTotal = reader.GetDecimal(2)
            },
            new[] { Param("id_confederacion", idConfederacion) },
            cancellationToken);

    public Task<List<PaisPorSedeReporteItem>> GetCountriesPlayingByHostCountryAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT DISTINCT anfitrion.nombre pais_anfitrion,
                   participante.nombre pais_participante
            FROM partido p
            INNER JOIN estadio es ON es.id_estadio = p.id_estadio
            INNER JOIN ciudad c ON c.id_ciudad = es.id_ciudad
            INNER JOIN pais anfitrion ON anfitrion.id_pais = c.id_pais
            INNER JOIN participacion part ON part.id_partido = p.id_partido
            INNER JOIN equipo e ON e.id_equipo = part.id_equipo
            INNER JOIN pais participante ON participante.id_pais = e.id_pais
            WHERE anfitrion.es_anfitrion = 1
            ORDER BY anfitrion.nombre, participante.nombre
            """,
            reader => new PaisPorSedeReporteItem
            {
                PaisAnfitrion = reader.GetString(0),
                PaisParticipante = reader.GetString(1)
            },
            cancellationToken: cancellationToken);
}
