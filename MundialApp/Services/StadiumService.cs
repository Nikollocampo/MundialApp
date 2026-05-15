using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class StadiumService(StadiumRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly StadiumRepository _repository = repository;

    public Task<List<Estadio>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public async Task SaveAsync(Estadio item, CancellationToken cancellationToken = default)
    {
        var esNuevo = item.IdEstadio == 0;
        var idEstadio = item.IdEstadio.ToString();

        await _repository.SaveAsync(item, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "ESTADIO",
                idEstadio,
                $"Estadio: {item.Nombre}, Capacidad: {item.Capacidad}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "ESTADIO",
                idEstadio,
                "Nombre, Ciudad, Capacidad",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var estadios = await _repository.GetAllAsync(cancellationToken);
        var estadio = estadios.FirstOrDefault(e => e.IdEstadio == id);

        await _repository.DeleteAsync(id, cancellationToken);

        if (estadio is not null)
        {
            await RegistrarEliminacionAsync(
                "ESTADIO",
                id.ToString(),
                $"Estadio: {estadio.Nombre}",
                cancellationToken);
        }
    }
}
