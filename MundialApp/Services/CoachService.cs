using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class CoachService(CoachRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly CoachRepository _repository = repository;

    public Task<List<DirectorTecnico>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public async Task SaveAsync(DirectorTecnico item, CancellationToken cancellationToken = default)
    {
        var esNuevo = item.IdDt == 0;
        var idDt = item.IdDt.ToString();

        await _repository.SaveAsync(item, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "DIRECTOR_TECNICO",
                idDt,
                $"DT: {item.Nombre}, Equipo: {item.Equipo}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "DIRECTOR_TECNICO",
                idDt,
                "Nombre, Nacionalidad, Equipo",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var directores = await _repository.GetAllAsync(cancellationToken);
        var director = directores.FirstOrDefault(d => d.IdDt == id);

        await _repository.DeleteAsync(id, cancellationToken);

        if (director is not null)
        {
            await RegistrarEliminacionAsync(
                "DIRECTOR_TECNICO",
                id.ToString(),
                $"DT: {director.Nombre}",
                cancellationToken);
        }
    }
}
