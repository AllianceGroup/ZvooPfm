using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Membership.Filters
{
    public class UserFilter : BaseFilter
    {
        public string UserNameOrEmail { get; set; }

        public string Id { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string AuthToken { get; set; }

        public string ResetPasswordToken { get; set; }

        public string Email { get; set; }

        public string SocialIdentity { get; set; }

        public string MobileAccessToken { get; set; }

        public string AffiliateId { get; set; }

        public string SearchKey { get; set; }

        public string AffiliateAdminSearchKey { get; set; }

        public int? SubscriptionId { get; set; }
        public bool IsAgent { get; set; }
        public bool IsCreatedByAgent { get; set; }
        public string CreatedBy { get; set; }
    }
}
