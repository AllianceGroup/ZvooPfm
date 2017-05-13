using System.Collections.Generic;
using System.Threading.Tasks;
using mPower.Domain.Membership.Enums;
using mPower.WebApi.Tenants.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Linq;

namespace mPower.WebApi.Authorization
{
    public class PermissionsAuthorizationHandler : IAuthorizationHandler
    {
        private readonly IUserPermissionService _userPermissionService;

        public PermissionsAuthorizationHandler(IUserPermissionService userPermissionService)
        {
            _userPermissionService = userPermissionService;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var castedContext = context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext;
            if (castedContext != null && !CheckPermissions(castedContext))
                context.Fail();

            return Task.CompletedTask;
        }

        public bool CheckPermissions(Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext context)
        {
            var requiredPermission = GetPermissions(context);
            if (requiredPermission.Length == 0)
                return true;

            var userId = GetUserId(context);
            if (string.IsNullOrEmpty(userId))
                return false;

            var actualPermissions = _userPermissionService.GetUserPermissions(userId);

            return requiredPermission.All(attributePermissions => HasAnyFitPermission(attributePermissions, actualPermissions));
        }

        private static bool HasAnyFitPermission(IEnumerable<UserPermissionEnum> required, IEnumerable<UserPermissionEnum> actual)
        {
            return actual.Any(required.Contains);
        }

        private static UserPermissionEnum[][] GetPermissions(ActionContext context)
        {
            return GetClassPermissions(context).Concat(GetMethodPermissions(context)).ToArray();
        }

        private static IEnumerable<UserPermissionEnum[]> GetMethodPermissions(ActionContext context)
        {
            var attributes = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttributes(true);
            return GetPermissionsFromAtributes(attributes);
        }

        private static IEnumerable<UserPermissionEnum[]> GetClassPermissions(ActionContext context)
        {
            var attributes = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerTypeInfo.GetCustomAttributes(true);
            return GetPermissionsFromAtributes(attributes);
        }

        private static IEnumerable<UserPermissionEnum[]> GetPermissionsFromAtributes(IEnumerable<object> attributes)
        {
            return attributes.Where(a => a is AllianceAuthorizeAttribute).Select(a => GetPermissionsFromAtribute((AllianceAuthorizeAttribute)a)).ToArray();
        }

        private static UserPermissionEnum[] GetPermissionsFromAtribute(AllianceAuthorizeAttribute authAttribute)
        {
            var roles = authAttribute.Permissions;
            var userPermissionEnums = roles as UserPermissionEnum[] ?? roles.ToArray();
            return !userPermissionEnums.Any() ? new UserPermissionEnum[0] : userPermissionEnums.Select(role => role).ToArray();
        }

        private static string GetUserId(ActionContext context)
        {
            return context.HttpContext.User.Identity?.Name;
        }
    }
}
