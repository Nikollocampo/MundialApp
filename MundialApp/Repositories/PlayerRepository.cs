using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class PlayerRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Jugador>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT j.id_jugador, j.nombre, j.id_equipo, j.posicion, j.fecha_nacimiento, j.edad,
                   j.costo, j.peso, j.altura, e.nombre equipo
            FROM jugador j
            INNER JOIN equipo e ON e.id_equipo = j.id_equipo
            ORDER BY e.nombre, j.nombre
            """,
            reader => new Jugador
            {
                IdJugador = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                IdEquipo = reader.GetInt32(2),
                Posicion = reader.GetString(3),
                FechaNacimiento = reader.GetDateTime(4),
                Edad = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                Costo = reader.GetDecimal(6),
                Peso = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                Altura = reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                Equipo = reader.GetString(9)
            },
            cancellationToken: cancellationToken);

    public Task SaveAsync(Jugador jugador, CancellationToken cancellationToken = default)
        => jugador.IdJugador == 0
            ? ExecuteAsync(
                """
                INSERT INTO jugador (nombre, id_equipo, posicion, fecha_nacimiento, edad, costo, peso, altura)
                VALUES (:nombre, :id_equipo, :posicion, :fecha_nacimiento, :edad, :costo, :peso, :altura)
                """,
                BuildParameters(jugador),
                cancellationToken)
            : ExecuteAsync(
                """
                UPDATE jugador
                SET nombre = :nombre,
                    id_equipo = :id_equipo,
                    posicion = :posicion,
                    fecha_nacimiento = :fecha_nacimiento,
                    edad = :edad,
                    costo = :costo,
                    peso = :peso,
                    altura = :altura
                WHERE id_jugador = :id_jugador
                """,
                BuildParameters(jugador, true),
                cancellationToken);

    public Task DeleteAsync(int idJugador, CancellationToken cancellationToken = default)
        => ExecuteAsync("DELETE FROM jugador WHERE id_jugador = :id_jugador", new[] { Param("id_jugador", idJugador) }, cancellationToken);

    private IEnumerable<Oracle.ManagedDataAccess.Client.OracleParameter> BuildParameters(Jugador jugador, bool includeId = false)
    {
        var parameters = new List<Oracle.ManagedDataAccess.Client.OracleParameter>
        {
            Param("nombre", jugador.Nombre),
            Param("id_equipo", jugador.IdEquipo),
            Param("posicion", jugador.Posicion),
            Param("fecha_nacimiento", jugador.FechaNacimiento),
            Param("edad", jugador.Edad),
            Param("costo", jugador.Costo),
            Param("peso", jugador.Peso),
            Param("altura", jugador.Altura)
        };

        if (includeId)
        {
            parameters.Add(Param("id_jugador", jugador.IdJugador));
        }

        return parameters;
    }
}
