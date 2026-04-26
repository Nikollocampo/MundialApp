namespace MundialApp.Models.Dto;

public sealed class JugadorCostosoPorConfederacionDto
{
    public string Confederacion { get; set; } = string.Empty;
    public string Jugador { get; set; } = string.Empty;
    public string Equipo { get; set; } = string.Empty;
    public decimal Costo { get; set; }
}

public sealed class PartidoPorEstadioDto
{
    public int IdPartido { get; set; }
    public string Estadio { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string EquipoLocal { get; set; } = string.Empty;
    public string EquipoVisitante { get; set; } = string.Empty;
}

public sealed class EquipoCostosoPorPaisAnfitrionDto
{
    public string PaisAnfitrion { get; set; } = string.Empty;
    public string Equipo { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
}

public sealed class CantidadJugadoresJovenesDto
{
    public string Equipo { get; set; } = string.Empty;
    public int CantidadMenoresDe21 { get; set; }
}
