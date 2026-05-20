using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class LookupRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<LookupItem>> GetGruposAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_grupo, nombre FROM grupo ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetPaisesAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_pais, nombre FROM pais ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetConfederacionesAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_confederacion, nombre FROM confederacion ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetEquiposAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_equipo, nombre FROM equipo ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetEstadiosAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_estadio, nombre FROM estadio ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetCiudadesAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_ciudad, nombre FROM ciudad ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetJugadoresAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_jugador, nombre FROM jugador ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetPartidosAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT p.id_partido,
                   TO_CHAR(p.fecha, 'DD/MM/YYYY HH24:MI') || ' - ' ||
                   COALESCE(el.nombre, 'Local ?') || ' vs ' ||
                   COALESCE(ev.nombre, 'Visitante ?') AS nombre
            FROM partido p
            LEFT JOIN participacion pl ON pl.id_partido = p.id_partido AND pl.condicion = 'LOCAL'
            LEFT JOIN equipo el ON el.id_equipo = pl.id_equipo
            LEFT JOIN participacion pv ON pv.id_partido = p.id_partido AND pv.condicion = 'VISITANTE'
            LEFT JOIN equipo ev ON ev.id_equipo = pv.id_equipo
            ORDER BY p.fecha DESC
            """,
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<Jugador>> GetJugadoresPorEquipoAsync(int idEquipo, CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT j.id_jugador, j.nombre, j.id_equipo, j.posicion, j.fecha_nacimiento,
                   j.costo, j.peso, j.altura, e.nombre equipo
            FROM jugador j
            INNER JOIN equipo e ON e.id_equipo = j.id_equipo
            WHERE j.id_equipo = :id_equipo
            ORDER BY j.nombre
            """,
            reader => new Jugador
            {
                IdJugador = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                IdEquipo = reader.GetInt32(2),
                Posicion = reader.GetString(3),
                FechaNacimiento = reader.GetDateTime(4),
                Costo = reader.GetDecimal(5),
                Peso = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                Altura = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                Equipo = reader.GetString(8)
            },
            [Param("id_equipo", idEquipo)],
            cancellationToken: cancellationToken);

    public Task<Participacion?> GetParticipacionAsync(int idPartido, int idEquipo, CancellationToken cancellationToken = default)
        => QuerySingleAsync(
            """
            SELECT p.codigo, p.id_equipo, p.id_partido, p.condicion, p.sanciones
            FROM participacion p
            WHERE p.id_partido = :id_partido AND p.id_equipo = :id_equipo
            """,
            reader => new Participacion
            {
                Codigo = reader.GetInt32(0),
                IdEquipo = reader.GetInt32(1),
                IdPartido = reader.GetInt32(2),
                Condicion = reader.GetString(3),
                Sanciones = reader.IsDBNull(4) ? null : reader.GetString(4)
            },
            [Param("id_partido", idPartido), Param("id_equipo", idEquipo)],
            cancellationToken: cancellationToken);
}
