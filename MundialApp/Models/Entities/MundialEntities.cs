namespace MundialApp.Models.Entities;

public sealed class Confederacion
{
    public int IdConfederacion { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CuposMundial { get; set; }
    public DateTime? FechaCreacion { get; set; }
}

public sealed class Pais
{
    public int IdPais { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int? Msnm { get; set; }
    public bool EsAnfitrion { get; set; }
}

public sealed class Ciudad
{
    public int IdCiudad { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdPais { get; set; }
    public string? Pais { get; set; }
}

public sealed class Estadio
{
    public int IdEstadio { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdCiudad { get; set; }
    public int Capacidad { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
}

public sealed class Grupo
{
    public int IdGrupo { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public sealed class Equipo
{
    public int IdEquipo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Escudo { get; set; }
    public int IdGrupo { get; set; }
    public int IdPais { get; set; }
    public int IdConfederacion { get; set; }
    public string? Grupo { get; set; }
    public string? Pais { get; set; }
    public string? Confederacion { get; set; }
}

public sealed class DirectorTecnico
{
    public int IdDt { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Nacionalidad { get; set; }
    public int IdEquipo { get; set; }
    public string? Equipo { get; set; }
}

public sealed class Jugador
{
    public int IdJugador { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdEquipo { get; set; }
    public string Posicion { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; } = DateTime.Today;
    public int? Edad { get; set; }
    public decimal Costo { get; set; }
    public decimal? Peso { get; set; }
    public decimal? Altura { get; set; }
    public string? Equipo { get; set; }
}

public sealed class Partido
{
    public int IdPartido { get; set; }
    public int IdEstadio { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public int IdLocal { get; set; }
    public int IdVisitante { get; set; }
    public string? EquipoLocal { get; set; }
    public string? EquipoVisitante { get; set; }
    public string? Estadio { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
}

public sealed class Usuario
{
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Rol { get; set; } = string.Empty;
}

public sealed class Bitacora
{
    public int IdBitacora { get; set; }
    public string IdUsuario { get; set; } = string.Empty;
    public DateTime FechaEntrada { get; set; }
    public DateTime? FechaSalida { get; set; }
    public string? Accion { get; set; }
}

public sealed class LookupItem
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
