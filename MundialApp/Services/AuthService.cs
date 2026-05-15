using MundialApp.Models.Dto;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class AuthService(
    AuthRepository authRepository,
    SessionState sessionState,
    AuditService auditService)
{
    private readonly AuthRepository _authRepository = authRepository;
    private readonly SessionState _sessionState = sessionState;
    private readonly AuditService _auditService = auditService;

    public async Task<LoginResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _authRepository.GetByIdentifierAsync(
            request.Identificador.Trim(),
            cancellationToken);

        if (usuario is null)
        {
            return new LoginResult
            {
                Success = false,
                Message = "Usuario no encontrado."
            };
        }

        if (!string.Equals(
            usuario.Contrasena,
            request.Contrasena,
            StringComparison.Ordinal))
        {
            return new LoginResult
            {
                Success = false,
                Message = "Contraseña inválida."
            };
        }

        // Guardar usuario en sesión
        _sessionState.SetSession(usuario);

        // Registrar LOGIN en auditoría
        await _auditService.RegistrarAuditoriaAsync(
            usuario.Cedula,
            "AUTENTICACION",
            "LOGIN",
            $"Usuario {usuario.Nombre} ({usuario.Rol}) inició sesión",
            usuario.Cedula,
            cancellationToken);

        return new LoginResult
        {
            Success = true,
            Message = $"Bienvenido {usuario.Nombre}",
            Usuario = usuario
        };
    }

    public async Task LogoutAsync(
        CancellationToken cancellationToken = default)
    {
        if (_sessionState.CurrentUser is not null)
        {
            var usuarioId = _sessionState.CurrentUser.Cedula;

            // Registrar LOGOUT en auditoría
            await _auditService.RegistrarAuditoriaAsync(
                usuarioId,
                "AUTENTICACION",
                "LOGOUT",
                $"Usuario {_sessionState.CurrentUser.Nombre} cerró sesión",
                usuarioId,
                cancellationToken);
        }

        // Limpiar sesión
        _sessionState.Clear();
    }
}