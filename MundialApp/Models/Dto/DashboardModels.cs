using MundialApp.Models.Entities;

namespace MundialApp.Models.Dto;

public sealed class DashboardSummary
{
    public int TotalEquipos { get; set; }
    public int TotalJugadores { get; set; }
    public int TotalPartidos { get; set; }
    public int TotalEstadios { get; set; }
    public int TotalUsuarios { get; set; }
    public List<Partido> ProximosPartidos { get; set; } = new();
    public List<string> PaisesAnfitriones { get; set; } = new();
}
