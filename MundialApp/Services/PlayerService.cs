using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class PlayerService(PlayerRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly PlayerRepository _repository = repository;

    public Task<List<Jugador>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public async Task SaveAsync(Jugador jugador, CancellationToken cancellationToken = default)
    {
        var esNuevo = jugador.IdJugador == 0;
        var idJugador = jugador.IdJugador.ToString();

        await _repository.SaveAsync(jugador, cancellationToken);

        // Registrar auditoría después de guardar exitosamente
        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "JUGADOR",
                idJugador,
                $"Jugador: {jugador.Nombre}, Posición: {jugador.Posicion}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "JUGADOR",
                idJugador,
                "Nombre, Posición, Fecha de Nacimiento, Costo, Peso, Altura",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(int idJugador, CancellationToken cancellationToken = default)
    {
        // Obtener info del jugador ANTES de eliminarlo para la auditoría
        var jugadores = await _repository.GetAllAsync(cancellationToken);
        var jugador = jugadores.FirstOrDefault(j => j.IdJugador == idJugador);

        await _repository.DeleteAsync(idJugador, cancellationToken);

        // Registrar auditoría después de eliminar exitosamente
        if (jugador is not null)
        {
            await RegistrarEliminacionAsync(
                "JUGADOR",
                idJugador.ToString(),
                $"Jugador: {jugador.Nombre}",
                cancellationToken);
        }
    }
}
