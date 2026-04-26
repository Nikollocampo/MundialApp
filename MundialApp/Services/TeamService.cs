using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class TeamService(TeamRepository repository)
{
    public Task<List<Equipo>> GetAllAsync(CancellationToken cancellationToken = default) => repository.GetAllAsync(cancellationToken);
    public Task SaveAsync(Equipo equipo, CancellationToken cancellationToken = default) => repository.SaveAsync(equipo, cancellationToken);
    public Task DeleteAsync(int idEquipo, CancellationToken cancellationToken = default) => repository.DeleteAsync(idEquipo, cancellationToken);
}
