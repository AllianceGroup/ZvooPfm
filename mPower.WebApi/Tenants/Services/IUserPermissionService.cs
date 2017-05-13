using mPower.Domain.Membership.Enums;

namespace mPower.WebApi.Tenants.Services
{
    public interface IUserPermissionService
    {
        UserPermissionEnum[] GetUserPermissions(string userId);
    }
}
