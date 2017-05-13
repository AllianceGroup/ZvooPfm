using mPower.Framework.Extensions;

namespace mPower.Domain.Membership.Enums
{
    public static class UserPermissionEnumExtension
    {
        public static bool IsAffiliateAdminPermission(this UserPermissionEnum permission)
        {
            return permission.In(UserPermissionEnum.AffiliateAdminView, UserPermissionEnum.AffiliateAdminEdit,
                UserPermissionEnum.AffiliateAdminDelete);
        }

        public static bool IsGlobalAdminPermission(this UserPermissionEnum permission)
        {
            return permission.In(UserPermissionEnum.GlobalAdminView, UserPermissionEnum.GlobalAdminEdit,
                UserPermissionEnum.GlobalAdminDelete);
        }
    }
}
