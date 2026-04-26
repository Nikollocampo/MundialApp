namespace MundialApp.Models.Security;

public static class UserRoles
{
    public const string Administrador = "ADMINISTRADOR";
    public const string Tradicional = "TRADICIONAL";
    public const string Esporadico = "ESPORADICO";

    public static bool CanEditData(string? role) => role is Administrador or Tradicional;
    public static bool CanManageUsers(string? role) => role == Administrador;
    public static bool CanQuery(string? role) => role is Administrador or Tradicional or Esporadico;
}
