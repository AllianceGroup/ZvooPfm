using System;
using System.Collections.Generic;
using mPower.Domain.Membership.Enums;
using Microsoft.AspNetCore.Authorization;

namespace mPower.WebApi.Authorization
{
    public class AllianceAuthorizeAttribute: AuthorizeAttribute
    {
        public readonly IEnumerable<UserPermissionEnum> Permissions;

        public AllianceAuthorizeAttribute(params UserPermissionEnum[] permissions) :base("Pfm")
        {
            if (permissions == null)
                throw new ArgumentNullException(nameof(permissions));

            Permissions = permissions;
        }
    }
}
