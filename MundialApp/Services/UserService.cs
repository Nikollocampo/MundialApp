using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class UserService(UserRepository repository, AuditService auditService, SessionState sessionState)
    : AuditedServiceBase(auditService, sessionState)
{
    private readonly UserRepository _repository = repository;

    public Task<List<Usuario>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _repository.GetAllAsync(cancellationToken);

    public async Task SaveAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        var esNuevo = string.IsNullOrEmpty(usuario.Cedula);
        var cedula = usuario.Cedula;

        await _repository.SaveAsync(usuario, cancellationToken);

        if (esNuevo)
        {
            await RegistrarInsercionAsync(
                "USUARIO",
                cedula,
                $"Usuario: {usuario.Nombre}, Rol: {usuario.Rol}",
                cancellationToken);
        }
        else
        {
            await RegistrarActualizacionAsync(
                "USUARIO",
                cedula,
                "Nombre, Correo, Teléfono, Rol",
                cancellationToken);
        }
    }

    public async Task DeleteAsync(string cedula, CancellationToken cancellationToken = default)
    {
        var usuarios = await _repository.GetAllAsync(cancellationToken);
        var usuario = usuarios.FirstOrDefault(u => u.Cedula == cedula);

        await _repository.DeleteAsync(cedula, cancellationToken);

        if (usuario is not null)
        {
            await RegistrarEliminacionAsync(
                "USUARIO",
                cedula,
                $"Usuario: {usuario.Nombre}",
                cancellationToken);
        }
    }
}
