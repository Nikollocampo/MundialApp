using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class PlayerService(PlayerRepository repository)
{
    public Task<List<Jugador>> GetAllAsync(CancellationToken cancellationToken = default) => repository.GetAllAsync(cancellationToken);
    public Task SaveAsync(Jugador jugador, CancellationToken cancellationToken = default) => repository.SaveAsync(jugador, cancellationToken);
    public Task DeleteAsync(int idJugador, CancellationToken cancellationToken = default) => repository.DeleteAsync(idJugador, cancellationToken);
}
