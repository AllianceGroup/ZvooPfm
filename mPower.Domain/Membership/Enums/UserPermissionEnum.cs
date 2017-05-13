namespace mPower.Domain.Membership.Enums
{
    public enum UserPermissionEnum
    {
        /// <summary>
        /// View business finance management permission
        /// </summary>
        ViewPfm = 1,
        AffiliateAdminView = 10,
        AffiliateAdminEdit = 11,
        AffiliateAdminDelete = 12,
        GlobalAdminView = 13,
        GlobalAdminEdit = 14,
        GlobalAdminDelete = 15
    }

    public enum ApplicationsEnum
    {
        Pfm,
        Bfm,
        CreditApp,
        Marketing
    }
}
