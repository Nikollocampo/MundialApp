using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class GroupService(GroupRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly GroupRepository _repository = repository;

    public Task<List<Grupo>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public async Task SaveAsync(Grupo item, CancellationToken cancellationToken = default)
    {
        var esNuevo = item.IdGrupo == 0;
        var idGrupo = item.IdGrupo.ToString();

        await _repository.SaveAsync(item, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "GRUPO",
                idGrupo,
                $"Grupo: {item.Nombre}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "GRUPO",
                idGrupo,
                "Nombre",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var grupos = await _repository.GetAllAsync(cancellationToken);
        var grupo = grupos.FirstOrDefault(g => g.IdGrupo == id);

        await _repository.DeleteAsync(id, cancellationToken);

        if (grupo is not null)
        {
            await RegistrarEliminacionAsync(
                "GRUPO",
                id.ToString(),
                $"Grupo: {grupo.Nombre}",
                cancellationToken);
        }
    }
}
