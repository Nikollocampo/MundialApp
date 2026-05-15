using MundialApp.Models.Entities;
using MundialApp.Repositories.Infrastructure;

namespace MundialApp.Repositories;

public sealed class PlayerRepository(IOracleConnectionFactory connectionFactory) : OracleRepositoryBase(connectionFactory)
{
    public Task<List<Jugador>> GetAllAsync(CancellationToken cancellationToken = default)
        => QueryAsync(
            """
            SELECT j.id_jugador, j.nombre, j.id_equipo, j.posicion, j.fecha_nacimiento,
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
                Costo = reader.GetDecimal(5),
                Peso = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                Altura = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                Equipo = reader.GetString(8)
            },
            cancellationToken: cancellationToken);

    public Task SaveAsync(Jugador jugador, CancellationToken cancellationToken = default)
    {
        NormalizeAndValidate(jugador);

        return jugador.IdJugador == 0
            ? ExecuteAsync(
                """
                INSERT INTO jugador (nombre, id_equipo, posicion, fecha_nacimiento, costo, peso, altura)
                VALUES (:nombre, :id_equipo, :posicion, :fecha_nacimiento, :costo, :peso, :altura)
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
                    costo = :costo,
                    peso = :peso,
                    altura = :altura
                WHERE id_jugador = :id_jugador
                """,
                BuildParameters(jugador, true),
                cancellationToken);
    }

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

    private static void NormalizeAndValidate(Jugador jugador)
    {
        jugador.Costo = Math.Round(jugador.Costo, 2, MidpointRounding.AwayFromZero);
        jugador.Peso = jugador.Peso is decimal peso
            ? Math.Round(peso, 2, MidpointRounding.AwayFromZero)
            : null;
        jugador.Altura = jugador.Altura is decimal altura
            ? Math.Round(altura, 2, MidpointRounding.AwayFromZero)
            : null;

        if (jugador.Costo is < 0 or > 9999999999999.99m)
        {
            throw new InvalidOperationException("El costo excede el tamaño permitido por la base de datos.");
        }

        if (jugador.Peso is < 0 or > 999.99m)
        {
            throw new InvalidOperationException("El peso debe tener máximo 3 enteros y 2 decimales.");
        }

        if (jugador.Altura is < 0 or > 99.99m)
        {
            throw new InvalidOperationException("La altura debe tener máximo 2 enteros y 2 decimales.");
        }
    }
}
