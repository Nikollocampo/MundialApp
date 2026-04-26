using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class ConfederationRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Confederacion>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            "SELECT id_confederacion, nombre, cupos_mundial, fecha_creacion FROM confederacion ORDER BY nombre",
            reader => new Confederacion
            {
                IdConfederacion = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                CuposMundial = reader.GetInt32(2),
                FechaCreacion = reader.IsDBNull(3) ? null : reader.GetDateTime(3)
            },
            cancellationToken: cancellationToken);

    public Task SaveAsync(Confederacion item, CancellationToken cancellationToken = default)
        => item.IdConfederacion == 0
            ? ExecuteAsync(
                "INSERT INTO confederacion (nombre, cupos_mundial, fecha_creacion) VALUES (:nombre, :cupos_mundial, :fecha_creacion)",
                new[]
                {
                    Param("nombre", item.Nombre),
                    Param("cupos_mundial", item.CuposMundial),
                    Param("fecha_creacion", item.FechaCreacion ?? DateTime.Today)
                },
                cancellationToken)
            : ExecuteAsync(
                "UPDATE confederacion SET nombre = :nombre, cupos_mundial = :cupos_mundial, fecha_creacion = :fecha_creacion WHERE id_confederacion = :id_confederacion",
                new[]
                {
                    Param("nombre", item.Nombre),
                    Param("cupos_mundial", item.CuposMundial),
                    Param("fecha_creacion", item.FechaCreacion ?? DateTime.Today),
                    Param("id_confederacion", item.IdConfederacion)
                },
                cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM confederacion WHERE id_confederacion = :id", new[] { Param("id", id) }, cancellationToken);
}
