using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class CityService(CityRepository repository)
{
    public Task<List<Ciudad>> GetAllAsync(CancellationToken cancellationToken = default) => repository.GetAllAsync(cancellationToken);
    public Task SaveAsync(Ciudad item, CancellationToken cancellationToken = default) => repository.SaveAsync(item, cancellationToken);
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) => repository.DeleteAsync(id, cancellationToken);
}
