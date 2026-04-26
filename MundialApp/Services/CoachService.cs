using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class CoachService(CoachRepository repository)
{
    public Task<List<DirectorTecnico>> GetAllAsync(CancellationToken cancellationToken = default) => repository.GetAllAsync(cancellationToken);
    public Task SaveAsync(DirectorTecnico item, CancellationToken cancellationToken = default) => repository.SaveAsync(item, cancellationToken);
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) => repository.DeleteAsync(id, cancellationToken);
}
