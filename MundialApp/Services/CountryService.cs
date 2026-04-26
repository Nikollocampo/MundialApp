using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class CountryService(CountryRepository repository)
{
    public Task<List<Pais>> GetAllAsync(CancellationToken cancellationToken = default) => repository.GetAllAsync(cancellationToken);
    public Task SaveAsync(Pais item, CancellationToken cancellationToken = default) => repository.SaveAsync(item, cancellationToken);
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) => repository.DeleteAsync(id, cancellationToken);
}
