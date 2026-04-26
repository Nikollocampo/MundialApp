using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class GroupRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Grupo>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_grupo, nombre FROM grupo ORDER BY nombre",
            reader => new Grupo
            {
                IdGrupo = reader.GetInt32(0),
                Nombre = reader.GetString(1)
            },
            cancellationToken: cancellationToken);

    public Task SaveAsync(Grupo item, CancellationToken cancellationToken = default)
        => item.IdGrupo == 0
            ? ExecuteAsync(
                "INSERT INTO grupo (nombre) VALUES (:nombre)",
                new[] { Param("nombre", item.Nombre) },
                cancellationToken)
            : ExecuteAsync(
                "UPDATE grupo SET nombre = :nombre WHERE id_grupo = :id_grupo",
                new[] { Param("nombre", item.Nombre), Param("id_grupo", item.IdGrupo) },
                cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM grupo WHERE id_grupo = :id", new[] { Param("id", id) }, cancellationToken);
}
