using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class TeamRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Equipo>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT e.id_equipo, e.nombre, e.escudo, e.id_grupo, e.id_pais, e.id_confederacion,
                   g.nombre grupo, p.nombre pais, c.nombre confederacion
            FROM equipo e
            INNER JOIN grupo g ON g.id_grupo = e.id_grupo
            INNER JOIN pais p ON p.id_pais = e.id_pais
            INNER JOIN confederacion c ON c.id_confederacion = e.id_confederacion
            ORDER BY e.nombre
            """,
            reader => new Equipo
            {
                IdEquipo = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Escudo = reader.IsDBNull(2) ? null : reader.GetString(2),
                IdGrupo = reader.GetInt32(3),
                IdPais = reader.GetInt32(4),
                IdConfederacion = reader.GetInt32(5),
                Grupo = reader.GetString(6),
                Pais = reader.GetString(7),
                Confederacion = reader.GetString(8)
            },
            cancellationToken: cancellationToken);

    public Task SaveAsync(Equipo equipo, CancellationToken cancellationToken = default)
        => equipo.IdEquipo == 0
            ? ExecuteAsync(
                """
                INSERT INTO equipo (nombre, escudo, id_grupo, id_pais, id_confederacion)
                VALUES (:nombre, :escudo, :id_grupo, :id_pais, :id_confederacion)
                """,
                BuildParameters(equipo),
                cancellationToken)
            : ExecuteAsync(
                """
                UPDATE equipo
                SET nombre = :nombre,
                    escudo = :escudo,
                    id_grupo = :id_grupo,
                    id_pais = :id_pais,
                    id_confederacion = :id_confederacion
                WHERE id_equipo = :id_equipo
                """,
                BuildParameters(equipo, true),
                cancellationToken);

    public Task DeleteAsync(int idEquipo, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM equipo WHERE id_equipo = :id_equipo", new[] { Param("id_equipo", idEquipo) }, cancellationToken);

    private IEnumerable<Oracle.ManagedDataAccess.Client.OracleParameter> BuildParameters(Equipo equipo, bool includeId = false)
    {
        var parameters = new List<Oracle.ManagedDataAccess.Client.OracleParameter>
        {
            Param("nombre", equipo.Nombre),
            Param("escudo", equipo.Escudo),
            Param("id_grupo", equipo.IdGrupo),
            Param("id_pais", equipo.IdPais),
            Param("id_confederacion", equipo.IdConfederacion)
        };

        if (includeId)
        {
            parameters.Add(Param("id_equipo", equipo.IdEquipo));
        }

        return parameters;
    }
}
