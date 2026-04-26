using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class MatchService(MatchRepository repository)
{
    public Task<List<Partido>> GetAllAsync(CancellationToken cancellationToken = default) => repository.GetAllAsync(cancellationToken);
    public Task SaveAsync(Partido partido, CancellationToken cancellationToken = default) => repository.SaveAsync(partido, cancellationToken);
    public Task DeleteAsync(int idPartido, CancellationToken cancellationToken = default) => repository.DeleteAsync(idPartido, cancellationToken);
}
