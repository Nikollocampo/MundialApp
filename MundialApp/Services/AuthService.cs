using MundialApp.Models.Dto;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class AuthService(AuthRepository authRepository, SessionState sessionState)
{
    private readonly AuthRepository _authRepository = authRepository;
    private readonly SessionState _sessionState = sessionState;

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _authRepository.GetByIdentifierAsync(request.Identificador.Trim(), cancellationToken);
        if (usuario is null)
        {
            return new LoginResult { Success = false, Message = "Usuario no encontrado." };
        }

        if (!string.Equals(usuario.Contrasena, request.Contrasena, StringComparison.Ordinal))
        {
            return new LoginResult { Success = false, Message = "Contraseña inválida." };
        }

        var bitacoraId = await _authRepository.RegisterLoginAsync(usuario.Cedula, cancellationToken);
        _sessionState.SetSession(usuario, bitacoraId);

        return new LoginResult
        {
            Success = true,
            Message = $"Bienvenido {usuario.Nombre}",
            Usuario = usuario,
            BitacoraId = bitacoraId
        };
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        if (_sessionState.CurrentBitacoraId is int bitacoraId)
        {
            await _authRepository.RegisterLogoutAsync(bitacoraId, cancellationToken);
        }

        _sessionState.Clear();
    }
}
