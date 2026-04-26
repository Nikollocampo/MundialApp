using MundialApp.Models.Entities;
using MundialApp.Repositories;

namespace MundialApp.Services;

public sealed class UserService(UserRepository repository)
{
    public Task<List<Usuario>> GetAllAsync(CancellationToken cancellationToken = default) => repository.GetAllAsync(cancellationToken);
    public Task SaveAsync(Usuario usuario, CancellationToken cancellationToken = default) => repository.SaveAsync(usuario, cancellationToken);
    public Task DeleteAsync(string cedula, CancellationToken cancellationToken = default) => repository.DeleteAsync(cedula, cancellationToken);
}
