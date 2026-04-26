using MundialApp.Models.Dto;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class DashboardService(DashboardRepository repository)
{
    public Task<DashboardSummary> GetSummaryAsync(CancellationToken cancellationToken = default)
        => repository.GetSummaryAsync(cancellationToken);
}
