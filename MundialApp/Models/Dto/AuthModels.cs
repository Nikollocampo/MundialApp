using MundialApp.Models.Entities;

namespace MundialApp.Models.Dto;

public sealed class LoginRequest
{
    public string Identificador { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
}

public sealed class LoginResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Usuario? Usuario { get; set; }
    public int? BitacoraId { get; set; }
}
