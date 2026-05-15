using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class CountryService(CountryRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly CountryRepository _repository = repository;

    public Task<List<Pais>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public async Task SaveAsync(Pais item, CancellationToken cancellationToken = default)
    {
        var esNuevo = item.IdPais == 0;
        var idPais = item.IdPais.ToString();

        await _repository.SaveAsync(item, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "PAIS",
                idPais,
                $"País: {item.Nombre}, Anfitrión: {item.EsAnfitrion}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "PAIS",
                idPais,
                "Nombre, MSNM, Es Anfitrión",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var paises = await _repository.GetAllAsync(cancellationToken);
        var pais = paises.FirstOrDefault(p => p.IdPais == id);

        await _repository.DeleteAsync(id, cancellationToken);

        if (pais is not null)
        {
            await RegistrarEliminacionAsync(
                "PAIS",
                id.ToString(),
                $"País: {pais.Nombre}",
                cancellationToken);
        }
    }
}
