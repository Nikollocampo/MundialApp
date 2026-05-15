using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class ConfederationService(ConfederationRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly ConfederationRepository _repository = repository;

    public Task<List<Confederacion>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public async Task SaveAsync(Confederacion item, CancellationToken cancellationToken = default)
    {
        var esNuevo = item.IdConfederacion == 0;
        var idConfederacion = item.IdConfederacion.ToString();

        await _repository.SaveAsync(item, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "CONFEDERACION",
                idConfederacion,
                $"Confederación: {item.Nombre}, Cupos: {item.CuposMundial}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "CONFEDERACION",
                idConfederacion,
                "Nombre, Cupos Mundial",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var confederaciones = await _repository.GetAllAsync(cancellationToken);
        var confederacion = confederaciones.FirstOrDefault(c => c.IdConfederacion == id);

        await _repository.DeleteAsync(id, cancellationToken);

        if (confederacion is not null)
        {
            await RegistrarEliminacionAsync(
                "CONFEDERACION",
                id.ToString(),
                $"Confederación: {confederacion.Nombre}",
                cancellationToken);
        }
    }
}
