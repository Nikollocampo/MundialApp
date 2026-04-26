using MundialApp.Models.Dto;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class QueryService(QueryRepository repository)
{
    public Task<List<JugadorCostosoPorConfederacionDto>> GetMostExpensivePlayerByConfederationAsync(CancellationToken cancellationToken = default)
        => repository.GetMostExpensivePlayerByConfederationAsync(cancellationToken);

    public Task<List<PartidoPorEstadioDto>> GetMatchesByStadiumAsync(int idEstadio, CancellationToken cancellationToken = default)
        => repository.GetMatchesByStadiumAsync(idEstadio, cancellationToken);

    public Task<List<EquipoCostosoPorPaisAnfitrionDto>> GetMostExpensiveTeamByHostCountryAsync(CancellationToken cancellationToken = default)
        => repository.GetMostExpensiveTeamByHostCountryAsync(cancellationToken);

    public Task<List<CantidadJugadoresJovenesDto>> GetYoungPlayersCountByTeamAsync(CancellationToken cancellationToken = default)
        => repository.GetYoungPlayersCountByTeamAsync(cancellationToken);
}
