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
        GlobalAdminDelete = 15,
        Agent = 16,
        //AgentEdit = 17,
        //AgentDelete = 18,
    }

    public enum ApplicationsEnum
    {
        Pfm,
        Bfm,
        CreditApp,
        Marketing
    }
}
