using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class LesionService(LesionRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly LesionRepository _repository = repository;

    public Task<List<Lesion>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public Task<List<Lesion>> GetByJugadorAsync(int idJugador, CancellationToken cancellationToken = default) 
        => _repository.GetByJugadorAsync(idJugador, cancellationToken);

    public async Task SaveAsync(Lesion lesion, CancellationToken cancellationToken = default)
    {
        var esNuevo = lesion.IdLesion == 0;
        var idLesion = lesion.IdLesion.ToString();

        await _repository.SaveAsync(lesion, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "LESION",
                idLesion,
                $"Jugador: {lesion.Jugador?.Nombre}, Descripción: {lesion.Descripcion}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "LESION",
                idLesion,
                "Descripción, Fecha de Inicio",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int idLesion, CancellationToken cancellationToken = default)
    {
        var lesiones = await _repository.GetAllAsync(cancellationToken);
        var lesion = lesiones.FirstOrDefault(l => l.IdLesion == idLesion);

        await _repository.DeleteAsync(idLesion, cancellationToken);

        if (lesion is not null)
        {
            await RegistrarEliminacionAsync(
                "LESION",
                idLesion.ToString(),
                $"Jugador: {lesion.Jugador?.Nombre}",
                cancellationToken);
        }
    }
}
