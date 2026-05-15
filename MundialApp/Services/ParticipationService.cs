using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class ParticipationService(ParticipationRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly ParticipationRepository _repository = repository;

    public Task<List<Participacion>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public Task<List<Participacion>> GetByPartidoAsync(int idPartido, CancellationToken cancellationToken = default) 
        => _repository.GetByPartidoAsync(idPartido, cancellationToken);

    public async Task SaveAsync(Participacion participacion, CancellationToken cancellationToken = default)
    {
        var esNuevo = participacion.Codigo == 0;
        var idParticipacion = participacion.Codigo.ToString();

        await _repository.SaveAsync(participacion, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "PARTICIPACION",
                idParticipacion,
                $"Equipo: {participacion.Equipo?.Nombre}, Condición: {participacion.Condicion}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "PARTICIPACION",
                idParticipacion,
                "Sanciones, Condición",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int codigo, CancellationToken cancellationToken = default)
    {
        var participaciones = await _repository.GetAllAsync(cancellationToken);
        var participacion = participaciones.FirstOrDefault(p => p.Codigo == codigo);

        await _repository.DeleteAsync(codigo, cancellationToken);

        if (participacion is not null)
        {
            await RegistrarEliminacionAsync(
                "PARTICIPACION",
                codigo.ToString(),
                $"Equipo: {participacion.Equipo?.Nombre}",
                cancellationToken);
        }
    }
}
