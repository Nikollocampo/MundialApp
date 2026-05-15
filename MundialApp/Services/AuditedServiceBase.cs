using MundialApp.Repositories;

namespace MundialApp.Services;

/// <summary>
/// Clase base para servicios que deben registrar auditoría automáticamente
/// Reutiliza código y evita duplicación en todos los formularios
/// </summary>
public abstract class AuditedServiceBase(AuditService auditService, SessionState sessionState)
{
    protected readonly AuditService AuditService = auditService;
    protected readonly SessionState SessionState = sessionState;

    /// <summary>
    /// Registra una inserción en la auditoría
    /// Llama esto después de insertar un registro exitosamente
    /// </summary>
    protected async Task RegistrarInsercionAsync(
        string tablaAfectada,
        string idRegistro,
        string descripcionAdicional = "",
        CancellationToken cancellationToken = default)
    {
        if (SessionState.CurrentUser is null)
            return;

        var descripcion = $"Registro insertado en {tablaAfectada}";
        if (!string.IsNullOrWhiteSpace(descripcionAdicional))
            descripcion += $": {descripcionAdicional}";

        await AuditService.RegistrarAuditoriaAsync(
            SessionState.CurrentUser.Cedula,
            tablaAfectada,
            "INSERT",
            descripcion,
            idRegistro,
            cancellationToken);
    }

    /// <summary>
    /// Registra una actualización en la auditoría
    /// Llama esto después de actualizar un registro exitosamente
    /// </summary>
    protected async Task RegistrarActualizacionAsync(
        string tablaAfectada,
        string idRegistro,
        string camposModificados,
        CancellationToken cancellationToken = default)
    {
        if (SessionState.CurrentUser is null)
            return;

        var descripcion = $"Registro actualizado en {tablaAfectada}. Campos modificados: {camposModificados}";

        await AuditService.RegistrarAuditoriaAsync(
            SessionState.CurrentUser.Cedula,
            tablaAfectada,
            "UPDATE",
            descripcion,
            idRegistro,
            cancellationToken);
    }

    /// <summary>
    /// Registra una eliminación en la auditoría
    /// Llama esto después de eliminar un registro exitosamente
    /// </summary>
    protected async Task RegistrarEliminacionAsync(
        string tablaAfectada,
        string idRegistro,
        string descripcionAdicional = "",
        CancellationToken cancellationToken = default)
    {
        if (SessionState.CurrentUser is null)
            return;

        var descripcion = $"Registro eliminado de {tablaAfectada}";
        if (!string.IsNullOrWhiteSpace(descripcionAdicional))
            descripcion += $": {descripcionAdicional}";

        await AuditService.RegistrarAuditoriaAsync(
            SessionState.CurrentUser.Cedula,
            tablaAfectada,
            "DELETE",
            descripcion,
            idRegistro,
            cancellationToken);
    }
}
