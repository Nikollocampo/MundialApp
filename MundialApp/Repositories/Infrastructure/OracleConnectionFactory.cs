using Microsoft.Extensions.Options;
using MundialApp.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace MundialApp.Repositories.Infrastructure;

public sealed class OracleConnectionFactory(IOptions<OracleOptions> options) : IOracleConnectionFactory
{
    private readonly OracleOptions _options = options.Value;

    public async Task<OracleConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new OracleConnection(_options.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
