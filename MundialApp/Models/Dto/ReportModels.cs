namespace MundialApp.Models.Dto;

public sealed class BitacoraReporteItem
{
    public string Usuario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaEntrada { get; set; }
    public DateTime? FechaSalida { get; set; }
    public string? Accion { get; set; }
}

public sealed class JugadorReporteItem
{
    public string Jugador { get; set; } = string.Empty;
    public decimal? Peso { get; set; }
    public decimal? Altura { get; set; }
    public string Equipo { get; set; } = string.Empty;
}

public sealed class ValorEquipoConfederacionReporteItem
{
    public string Confederacion { get; set; } = string.Empty;
    public string Equipo { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
}

public sealed class PaisPorSedeReporteItem
{
    public string PaisAnfitrion { get; set; } = string.Empty;
    public string PaisParticipante { get; set; } = string.Empty;
}
