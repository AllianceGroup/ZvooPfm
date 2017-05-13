using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mPower.Domain.Membership.Enums;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Exceptions;
using mPower.Documents.DocumentServices.Membership;

namespace Default
{
    public class PermissionsAttribute : ActionFilterAttribute
    {
        private readonly UserPermissionEnum[] _permissions;

        private SessionContext _sessionContext { get; set; }

        public PermissionsAttribute(params UserPermissionEnum[] permissions)
        {
            this._permissions = permissions;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _sessionContext = TenantTools.Selector.TenantsContainer.GetInstance<SessionContext>();

            var usersService = TenantTools.Selector.TenantsContainer.GetInstance<UserDocumentService>();

            var sessionUserId = _sessionContext.UserId;

            var user = usersService.GetById(sessionUserId);

            var hasAccess = user.HasPermissions(_permissions);

            if (!hasAccess)
                throw new MpowerSecurityException("Access denied");
        }
    }
}