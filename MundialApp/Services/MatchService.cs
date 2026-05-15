using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class MatchService(MatchRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly MatchRepository _repository = repository;

    public Task<List<Partido>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public async Task SaveAsync(Partido partido, CancellationToken cancellationToken = default)
    {
        var esNuevo = partido.IdPartido == 0;
        var idPartido = partido.IdPartido.ToString();

        await _repository.SaveAsync(partido, cancellationToken);

        // Registrar auditoría después de guardar exitosamente
        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "PARTIDO",
                idPartido,
                $"Partido creado: {partido.EquipoLocal} vs {partido.EquipoVisitante}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "PARTIDO",
                idPartido,
                "Fecha, Hora, Local, Visitante, Estadio",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int idPartido, CancellationToken cancellationToken = default)
    {
        // Obtener info del partido ANTES de eliminarlo para la auditoría
        var partidos = await _repository.GetAllAsync(cancellationToken);
        var partido = partidos.FirstOrDefault(p => p.IdPartido == idPartido);

        await _repository.DeleteAsync(idPartido, cancellationToken);

        // Registrar auditoría después de eliminar exitosamente
        if (partido is not null)
        {
            await RegistrarEliminacionAsync(
                "PARTIDO",
                idPartido.ToString(),
                $"Partido: {partido.EquipoLocal} vs {partido.EquipoVisitante}",
                cancellationToken);
        }
    }
}
