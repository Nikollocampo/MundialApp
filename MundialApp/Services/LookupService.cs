using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class LookupService(LookupRepository repository)
{
    public Task<List<LookupItem>> GetGruposAsync(CancellationToken cancellationToken = default) => repository.GetGruposAsync(cancellationToken);
    public Task<List<LookupItem>> GetPaisesAsync(CancellationToken cancellationToken = default) => repository.GetPaisesAsync(cancellationToken);
    public Task<List<LookupItem>> GetConfederacionesAsync(CancellationToken cancellationToken = default) => repository.GetConfederacionesAsync(cancellationToken);
    public Task<List<LookupItem>> GetEquiposAsync(CancellationToken cancellationToken = default) => repository.GetEquiposAsync(cancellationToken);
    public Task<List<LookupItem>> GetEstadiosAsync(CancellationToken cancellationToken = default) => repository.GetEstadiosAsync(cancellationToken);
}
