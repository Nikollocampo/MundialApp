using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class TeamService(TeamRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly TeamRepository _repository = repository;

    public Task<List<Equipo>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public async Task SaveAsync(Equipo equipo, CancellationToken cancellationToken = default)
    {
        var esNuevo = equipo.IdEquipo == 0;
        var idEquipo = equipo.IdEquipo.ToString();

        await _repository.SaveAsync(equipo, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "EQUIPO",
                idEquipo,
                $"Equipo: {equipo.Nombre}, País: {equipo.Pais}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "EQUIPO",
                idEquipo,
                "Nombre, Grupo, País, Confederación",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int idEquipo, CancellationToken cancellationToken = default)
    {
        var equipos = await _repository.GetAllAsync(cancellationToken);
        var equipo = equipos.FirstOrDefault(e => e.IdEquipo == idEquipo);

        await _repository.DeleteAsync(idEquipo, cancellationToken);

        if (equipo is not null)
        {
            await RegistrarEliminacionAsync(
                "EQUIPO",
                idEquipo.ToString(),
                $"Equipo: {equipo.Nombre}",
                cancellationToken);
        }
    }
}
