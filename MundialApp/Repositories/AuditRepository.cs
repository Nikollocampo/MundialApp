using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class AuditRepository(IOracleConnectionFactory connectionFactory)
    : OracleRepositoryBase(connectionFactory)
{
    /// <summary>
    /// Inserta un registro de auditoría en la tabla BITACORA
    /// </summary>
    public Task InsertarAsync(Bitacora bitacora, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            """
            INSERT INTO bitacora (id_usuario, fecha_accion, tabla_afectada, tipo_accion, descripcion, id_registro)
            VALUES (:id_usuario, :fecha_accion, :tabla_afectada, :tipo_accion, :descripcion, :id_registro)
            """,
            [
                Param("id_usuario", bitacora.IdUsuario),
                Param("fecha_accion", bitacora.FechaAccion),
                Param("tabla_afectada", bitacora.TablaAfectada),
                Param("tipo_accion", bitacora.TipoAccion),
                Param("descripcion", bitacora.Descripcion),
                Param("id_registro", (object?)bitacora.IdRegistro ?? DBNull.Value)
            ],
            cancellationToken);

    /// <summary>
    /// Obtiene el historial completo de auditoría o filtrado
    /// </summary>
    public Task<List<Bitacora>> ObtenerHistorialAsync(
        string? usuarioId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        CancellationToken cancellationToken = default)
    {
        var sql = "SELECT id_bitacora, id_usuario, fecha_accion, tabla_afectada, tipo_accion, descripcion, id_registro FROM bitacora WHERE 1=1";
        var parametros = new List<Oracle.ManagedDataAccess.Client.OracleParameter>();

        if (!string.IsNullOrEmpty(usuarioId))
        {
            sql += " AND id_usuario = :id_usuario";
            parametros.Add(Param("id_usuario", usuarioId));
        }

        if (fechaDesde.HasValue)
        {
            sql += " AND fecha_accion >= :fecha_desde";
            parametros.Add(Param("fecha_desde", fechaDesde.Value));
        }

        if (fechaHasta.HasValue)
        {
            sql += " AND fecha_accion <= :fecha_hasta";
            parametros.Add(Param("fecha_hasta", fechaHasta.Value.AddDays(1)));
        }

        sql += " ORDER BY fecha_accion DESC";

        return QueryAsync(
            sql,
            reader => new Bitacora
            {
                IdBitacora = reader.GetInt32(0),
                IdUsuario = reader.GetString(1),
                FechaAccion = reader.GetDateTime(2),
                TablaAfectada = reader.GetString(3),
                TipoAccion = reader.GetString(4),
                Descripcion = reader.GetString(5),
                IdRegistro = reader.IsDBNull(6) ? null : reader.GetString(6)
            },
            parametros,
            cancellationToken);
    }

    /// <summary>
    /// Obtiene el historial de auditoría de una tabla específica
    /// </summary>
    public Task<List<Bitacora>> ObtenerHistorialPorTablaAsync(
        string tablaAfectada,
        CancellationToken cancellationToken = default)
    {
        return QueryAsync(
            """
            SELECT id_bitacora, id_usuario, fecha_accion, tabla_afectada, tipo_accion, descripcion, id_registro
            FROM bitacora
            WHERE tabla_afectada = :tabla_afectada
            ORDER BY fecha_accion DESC
            """,
            reader => new Bitacora
            {
                IdBitacora = reader.GetInt32(0),
                IdUsuario = reader.GetString(1),
                FechaAccion = reader.GetDateTime(2),
                TablaAfectada = reader.GetString(3),
                TipoAccion = reader.GetString(4),
                Descripcion = reader.GetString(5),
                IdRegistro = reader.IsDBNull(6) ? null : reader.GetString(6)
            },
            [Param("tabla_afectada", tablaAfectada)],
            cancellationToken);
    }

    /// <summary>
    /// Obtiene el historial de auditoría de un registro específico
    /// </summary>
    public Task<List<Bitacora>> ObtenerHistorialRegistroAsync(
        string tablaAfectada,
        string idRegistro,
        CancellationToken cancellationToken = default)
    {
        return QueryAsync(
            """
            SELECT id_bitacora, id_usuario, fecha_accion, tabla_afectada, tipo_accion, descripcion, id_registro
            FROM bitacora
            WHERE tabla_afectada = :tabla_afectada AND id_registro = :id_registro
            ORDER BY fecha_accion DESC
            """,
            reader => new Bitacora
            {
                IdBitacora = reader.GetInt32(0),
                IdUsuario = reader.GetString(1),
                FechaAccion = reader.GetDateTime(2),
                TablaAfectada = reader.GetString(3),
                TipoAccion = reader.GetString(4),
                Descripcion = reader.GetString(5),
                IdRegistro = reader.IsDBNull(6) ? null : reader.GetString(6)
            },
            [Param("tabla_afectada", tablaAfectada), Param("id_registro", idRegistro)],
            cancellationToken);
    }
}
