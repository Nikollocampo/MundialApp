using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class LookupRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<LookupItem>> GetGruposAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_grupo, nombre FROM grupo ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetPaisesAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_pais, nombre FROM pais ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetConfederacionesAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_confederacion, nombre FROM confederacion ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetEquiposAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_equipo, nombre FROM equipo ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetEstadiosAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_estadio, nombre FROM estadio ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);

    public Task<List<LookupItem>> GetCiudadesAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_ciudad, nombre FROM ciudad ORDER BY nombre",
            reader => new LookupItem { Id = reader.GetInt32(0), Nombre = reader.GetString(1) },
            cancellationToken: cancellationToken);
}
