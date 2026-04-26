using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class MatchRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Partido>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
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

    public async Task SaveAsync(Partido partido, CancellationToken cancellationToken = default)
    {
        if (partido.IdPartido == 0)
        {
            await InsertAsync(partido, cancellationToken);
            return;
        }

        await UpdateAsync(partido, cancellationToken);
    }

    public async Task DeleteAsync(int idPartido, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync("DELETE FROM resultado WHERE id_partido = :id_partido", new[] { Param("id_partido", idPartido) }, cancellationToken);
        await ExecuteAsync("DELETE FROM participacion WHERE id_partido = :id_partido", new[] { Param("id_partido", idPartido) }, cancellationToken);
        await ExecuteAsync("DELETE FROM partido WHERE id_partido = :id_partido", new[] { Param("id_partido", idPartido) }, cancellationToken);
    }

    private async Task InsertAsync(Partido partido, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO partido (id_estadio, fecha)
            VALUES (:id_estadio, :fecha)
            RETURNING id_partido INTO :id_partido
            """;

        var output = new Oracle.ManagedDataAccess.Client.OracleParameter("id_partido", Oracle.ManagedDataAccess.Client.OracleDbType.Int32)
        {
            Direction = System.Data.ParameterDirection.Output
        };

        await using var command = await CreateCommandAsync(sql, new[]
        {
            Param("id_estadio", partido.IdEstadio),
            Param("fecha", partido.Fecha),
            output
        }, cancellationToken);

        await command.ExecuteNonQueryAsync(cancellationToken);
        var idPartido = ((Oracle.ManagedDataAccess.Types.OracleDecimal)output.Value).ToInt32();
        await command.Connection!.DisposeAsync();

        await ExecuteAsync(
            "INSERT INTO participacion (id_equipo, id_partido, condicion) VALUES (:id_equipo, :id_partido, 'LOCAL')",
            new[] { Param("id_equipo", partido.IdLocal), Param("id_partido", idPartido) },
            cancellationToken);

        await ExecuteAsync(
            "INSERT INTO participacion (id_equipo, id_partido, condicion) VALUES (:id_equipo, :id_partido, 'VISITANTE')",
            new[] { Param("id_equipo", partido.IdVisitante), Param("id_partido", idPartido) },
            cancellationToken);
    }

    private async Task UpdateAsync(Partido partido, CancellationToken cancellationToken)
    {
        await ExecuteAsync(
            "UPDATE partido SET id_estadio = :id_estadio, fecha = :fecha WHERE id_partido = :id_partido",
            new[] { Param("id_estadio", partido.IdEstadio), Param("fecha", partido.Fecha), Param("id_partido", partido.IdPartido) },
            cancellationToken);

        await ExecuteAsync(
            "UPDATE participacion SET id_equipo = :id_equipo WHERE id_partido = :id_partido AND condicion = 'LOCAL'",
            new[] { Param("id_equipo", partido.IdLocal), Param("id_partido", partido.IdPartido) },
            cancellationToken);

        await ExecuteAsync(
            "UPDATE participacion SET id_equipo = :id_equipo WHERE id_partido = :id_partido AND condicion = 'VISITANTE'",
            new[] { Param("id_equipo", partido.IdVisitante), Param("id_partido", partido.IdPartido) },
            cancellationToken);
    }
}
