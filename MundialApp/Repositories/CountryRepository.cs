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

    public async Task SaveAsync(Pais item, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(item, cancellationToken);

        if (item.IdPais == 0)
        {
            await ExecuteAsync(
                "INSERT INTO pais (nombre, msnm, es_anfitrion) VALUES (:nombre, :msnm, :es_anfitrion)",
                new[]
                {
                    Param("nombre", item.Nombre.Trim()),
                    Param("msnm", item.Msnm),
                    Param("es_anfitrion", item.EsAnfitrion ? 1 : 0)
                },
                cancellationToken);

            return;
        }

        await ExecuteAsync(
            "UPDATE pais SET nombre = :nombre, msnm = :msnm, es_anfitrion = :es_anfitrion WHERE id_pais = :id_pais",
            new[]
            {
                Param("nombre", item.Nombre.Trim()),
                Param("msnm", item.Msnm),
                Param("es_anfitrion", item.EsAnfitrion ? 1 : 0),
                Param("id_pais", item.IdPais)
            },
            cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM pais WHERE id_pais = :id", new[] { Param("id", id) }, cancellationToken);

    private async Task ValidateAsync(Pais item, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(item.Nombre))
        {
            throw new InvalidOperationException("Debe ingresar el nombre del país.");
        }

        var exists = await QuerySingleAsync(
            "SELECT 1 FROM pais WHERE UPPER(TRIM(nombre)) = UPPER(TRIM(:nombre)) AND id_pais <> :id_pais",
            reader => reader.GetInt32(0),
            new[]
            {
                Param("nombre", item.Nombre),
                Param("id_pais", item.IdPais)
            },
            cancellationToken);

        if (exists == 1)
        {
            throw new InvalidOperationException("Ya existe un país registrado con ese nombre.");
        }
    }
}
