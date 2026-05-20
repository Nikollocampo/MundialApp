using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class GolService(GolRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly GolRepository _repository = repository;

    public Task<List<Gol>> GetByPartidoAsync(int idPartido, CancellationToken cancellationToken = default)
        => _repository.GetByPartidoAsync(idPartido, cancellationToken);

    public Task<List<Gol>> GetByEquipoPartidoAsync(int idPartido, int idEquipo, CancellationToken cancellationToken = default)
        => _repository.GetByEquipoPartidoAsync(idPartido, idEquipo, cancellationToken);

    public async Task SaveAsync(Gol gol, CancellationToken cancellationToken = default)
    {
        // Validar minuto está entre 1 y 120 (90 min + 30 min suplementarios)
        if (gol.Minuto < 1 || gol.Minuto > 120)
        {
            throw new ArgumentException("El minuto debe estar entre 1 y 120.");
        }

        var esNuevo = gol.IdGol == 0;
        var idGol = gol.IdGol.ToString();

        await _repository.SaveAsync(gol, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "GOL",
                idGol,
                $"Jugador: {gol.Jugador?.Nombre}, Equipo: {gol.Equipo?.Nombre}, Minuto: {gol.Minuto}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "GOL",
                idGol,
                $"Minuto: {gol.Minuto}",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int idGol, Gol golInfo, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(idGol, cancellationToken);

        await RegistrarEliminacionAsync(
            "GOL",
            idGol.ToString(),
            $"Jugador: {golInfo.Jugador?.Nombre}, Equipo: {golInfo.Equipo?.Nombre}, Minuto: {golInfo.Minuto}",
            cancellationToken);
    }

    public Task DeleteByPartidoAsync(int idPartido, CancellationToken cancellationToken = default)
        => _repository.DeleteByPartidoAsync(idPartido, cancellationToken);
}
