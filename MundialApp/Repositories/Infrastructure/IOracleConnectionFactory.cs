using Oracle.ManagedDataAccess.Client;

namespace MundialApp.Repositories.Infrastructure;

public interface IOracleConnectionFactory
{
    Task<OracleConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
