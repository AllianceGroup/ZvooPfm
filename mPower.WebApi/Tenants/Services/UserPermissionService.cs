using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Membership.Enums;

namespace mPower.WebApi.Tenants.Services
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly UserDocumentService _userDocumentService;

        public UserPermissionService(UserDocumentService userDocumentService)
        {
            _userDocumentService = userDocumentService;
        }

        public UserPermissionEnum[] GetUserPermissions(string userId)
        {
            var user = _userDocumentService.GetById(userId);
            return user?.Permissions.ToArray() ?? new UserPermissionEnum[0];
        }
    }
}
