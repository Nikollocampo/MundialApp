using MundialApp.Models.Entities;
using MundialApp.Models.Security;

namespace MundialApp.Services;

public sealed class SessionState
{
    public Usuario? CurrentUser { get; private set; }
    public int? CurrentBitacoraId { get; private set; }

    public bool IsAuthenticated => CurrentUser is not null;
    public bool CanEditData => UserRoles.CanEditData(CurrentUser?.Rol);
    public bool CanManageUsers => UserRoles.CanManageUsers(CurrentUser?.Rol);
    public bool CanQuery => UserRoles.CanQuery(CurrentUser?.Rol);

    public event Action? Changed;

    public void SetSession(Usuario user, int bitacoraId)
    {
        CurrentUser = user;
        CurrentBitacoraId = bitacoraId;
        Changed?.Invoke();
    }

    public void Clear()
    {
        CurrentUser = null;
        CurrentBitacoraId = null;
        Changed?.Invoke();
    }
}
