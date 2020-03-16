using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Application.Enums;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Data;

namespace mPower.Documents.Documents.Membership
{
    public class UserDocument
    {
        public UserDocument()
        {
            SecurityLevel = SecurityLevelEnum.LoginAndPassword;
            Permissions = new List<UserPermissionEnum>();
            Subscriptions = new List<SubscriptionDocument>();
            Identities = new List<IdentityDocument>();
            Notifications = new List<NotificationConfigDocument>();
            BillingsList = new List<BillingDocument>();
            Realestates = new List<RealestateDocument>();
            AdditionalEmails = new List<string>();
            Phones = new List<string>();
            GoalsLinkedAccount = new GoalsLinkedAccountDocument();
            Settings = new SettingsDocument();
            MerchantInfo = new MerchantData();
            AcceptedOffers = new List<RedeemedOfferDocument>();
        }

        [BsonId]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public DateTime? BirthDate { get; set; }

        public GenderEnum? Gender { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string AuthToken { get; set; }

        public string MobileAccessToken { get; set; }

        public string ResetPasswordToken { get; set; }

        public string PasswordQuestion { get; set; }

        public string PasswordAnswer { get; set; }

        public SecurityLevelEnum SecurityLevel { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime LastAutoUpdateDate { get; set; }

        public DateTime LastPasswordChangedDate { get; set; }

        public List<UserPermissionEnum> Permissions { get; set; }

        public bool HasPermissions(params UserPermissionEnum[] permissions)
        {
            return permissions.All(permission => Permissions.Contains(permission));
        }

        public string ApplicationId { get; set; }

        public List<SubscriptionDocument> Subscriptions { get; set; }

        public List<BillingDocument> BillingsList { get; set; }

        public List<IdentityDocument> Identities { get; set; }

        public YodleeUserInfoDocument YodleeUserInfo { get; set; }

        public bool IsEnrolled { get; set; }

        public List<NotificationConfigDocument> Notifications { get; set; }

        public List<RealestateDocument> Realestates { get; set; }

        public List<string> AdditionalEmails { get; set; }

        public List<string> Phones { get; set; }

        public GoalsLinkedAccountDocument GoalsLinkedAccount { get; set; }

        public SettingsDocument Settings { get; set; }

        public MerchantData MerchantInfo { get; set; }

        public BillingData BillingInfo { get; set; }

        public List<RedeemedOfferDocument> AcceptedOffers { get; set; }

        public string FullNameLowerCase
        {
            get { return FullName.ToLower(); }
            set { }
        }

        /// <summary>
        /// Affiliate name in lower case
        /// </summary>
        public string AffiliateName { get; set; }

        public string ZipCode { get; set; }

        public int AccountsAggregated { get; set; }

        public string ReferralCode { get; set; }

        public void AcceptOffer(string offerId, DateTime date, string offerAffiliateId, OfferTypeEnum offerType, long? offerValueInCents, float? offerValueInPerc)
        {
            if (AcceptedOffers.Any(x => x.Id == offerId))
            {
                return;
            }
            var doc = new RedeemedOfferDocument(offerId, date, offerAffiliateId, offerType, offerValueInCents, offerValueInPerc);
            AcceptedOffers.Add(doc);
        }

        public bool IsOfferAccepted(string id, string applicationId)
        {
            return AcceptedOffers.Any(x => x.Id == id && x.OfferAffiliateId == applicationId);
        }

        public bool IsAgent { get; set; }
        public bool IsCreatedByAgent { get; set; }
        public string CreatedBy { get; set; }
    }
}
