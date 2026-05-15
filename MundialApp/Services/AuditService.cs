using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

/// <summary>
/// Servicio centralizado para registrar auditoría de todas las operaciones del sistema
/// </summary>
public sealed class AuditService(AuditRepository repository)
{
    /// <summary>
    /// Registra una acción de auditoría en la bitácora
    /// </summary>
    /// <param name="usuarioId">ID del usuario que realiza la acción</param>
    /// <param name="tablaAfectada">Nombre de la tabla que se modificó</param>
    /// <param name="tipoAccion">Tipo de acción: INSERT, UPDATE, DELETE, LOGIN, LOGOUT</param>
    /// <param name="descripcion">Descripción detallada de la acción</param>
    /// <param name="idRegistro">ID del registro afectado (opcional)</param>
    public Task RegistrarAuditoriaAsync(
        string usuarioId,
        string tablaAfectada,
        string tipoAccion,
        string descripcion,
        string? idRegistro = null,
        CancellationToken cancellationToken = default)
    {
        var bitacora = new Bitacora
        {
            IdUsuario = usuarioId,
            FechaAccion = DateTime.Now,
            TablaAfectada = tablaAfectada,
            TipoAccion = tipoAccion,
            Descripcion = descripcion,
            IdRegistro = idRegistro
        };

        return repository.InsertarAsync(bitacora, cancellationToken);
    }

    /// <summary>
    /// Obtiene el historial de auditoría completo o filtrado por usuario
    /// </summary>
    public Task<List<Bitacora>> ObtenerHistorialAsync(
        string? usuarioId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        CancellationToken cancellationToken = default)
    {
        return repository.ObtenerHistorialAsync(usuarioId, fechaDesde, fechaHasta, cancellationToken);
    }

    /// <summary>
    /// Obtiene el historial de auditoría de una tabla específica
    /// </summary>
    public Task<List<Bitacora>> ObtenerHistorialPorTablaAsync(
        string tablaAfectada,
        CancellationToken cancellationToken = default)
    {
        return repository.ObtenerHistorialPorTablaAsync(tablaAfectada, cancellationToken);
    }

    /// <summary>
    /// Obtiene el historial de auditoría de un registro específico
    /// </summary>
    public Task<List<Bitacora>> ObtenerHistorialRegistroAsync(
        string tablaAfectada,
        string idRegistro,
        CancellationToken cancellationToken = default)
    {
        return repository.ObtenerHistorialRegistroAsync(tablaAfectada, idRegistro, cancellationToken);
    }
}
