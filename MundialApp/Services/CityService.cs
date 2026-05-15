using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class CityService(CityRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly CityRepository _repository = repository;

    public Task<List<Ciudad>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public async Task SaveAsync(Ciudad item, CancellationToken cancellationToken = default)
    {
        var esNuevo = item.IdCiudad == 0;
        var idCiudad = item.IdCiudad.ToString();

        await _repository.SaveAsync(item, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "CIUDAD",
                idCiudad,
                $"Ciudad: {item.Nombre}, País: {item.Pais}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "CIUDAD",
                idCiudad,
                "Nombre, País",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var ciudades = await _repository.GetAllAsync(cancellationToken);
        var ciudad = ciudades.FirstOrDefault(c => c.IdCiudad == id);

        await _repository.DeleteAsync(id, cancellationToken);

        if (ciudad is not null)
        {
            await RegistrarEliminacionAsync(
                "CIUDAD",
                id.ToString(),
                $"Ciudad: {ciudad.Nombre}",
                cancellationToken);
        }
    }
}
