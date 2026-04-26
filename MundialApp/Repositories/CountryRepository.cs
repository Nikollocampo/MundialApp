using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class CountryRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Pais>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_pais, nombre, msnm, es_anfitrion FROM pais ORDER BY nombre",
            reader => new Pais
            {
                IdPais = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Msnm = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                EsAnfitrion = reader.GetInt32(3) == 1
            },
            cancellationToken: cancellationToken);

    public Task SaveAsync(Pais item, CancellationToken cancellationToken = default)
        => item.IdPais == 0
            ? ExecuteAsync(
                "INSERT INTO pais (nombre, msnm, es_anfitrion) VALUES (:nombre, :msnm, :es_anfitrion)",
                new[]
                {
                    Param("nombre", item.Nombre),
                    Param("msnm", item.Msnm),
                    Param("es_anfitrion", item.EsAnfitrion ? 1 : 0)
                },
                cancellationToken)
            : ExecuteAsync(
                "UPDATE pais SET nombre = :nombre, msnm = :msnm, es_anfitrion = :es_anfitrion WHERE id_pais = :id_pais",
                new[]
                {
                    Param("nombre", item.Nombre),
                    Param("msnm", item.Msnm),
                    Param("es_anfitrion", item.EsAnfitrion ? 1 : 0),
                    Param("id_pais", item.IdPais)
                },
                cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM pais WHERE id_pais = :id", new[] { Param("id", id) }, cancellationToken);
}
