using mPower.Documents.Documents.Membership;
using mPower.Domain.Membership.Enums;
using System;
using System.Web;

namespace Default
{
    public interface ISessionContext
    {
        string UserId { get; set; }
        string UserEmail { get; set; }
        bool IsUserAuthorized { get; }
        bool HasUserAffiliateAdminRights { get; set; }
        bool HasUserGlobalAdminRights { get; set; }
        string LedgerId { get; set; }
        string LedgerName { get; set; }
        string ClientKey { get; set; }
        string ReferralCode { get; set; }
        bool AggregationLoggingEnabled { get; set; }


        void Create(UserDocument user);
        void Logout();

        string GetStringSessionValue(string key);
        void SetStringSessionValue(string key, string value);
        bool GetBoolSessionValue(string key);
        void SetBoolSessionValue(string key, bool value);
        void RemoveSessionValue(string key);
        void SetSessionValue(string key, object value);
        object GetSessionValue(string key);
    }

    /// <summary>
    /// SessionContext can be obtained only through the TenantsContainer,
    /// but can't be obtained as usual through the tenant container
    /// We should probably use session only from BaseController, 
    /// no need to send it through Constructur parameter because if it not working
    /// </summary>
    [Serializable]
    public class SessionContext : ISessionContext
    {
        private const string UserIdKey = "UserId";
        private const string UserEmailKey = "UserEmail";
        private const string HasUserAffiliateAdminRightsKey = "HasUserAffiliateAdminRights";
        private const string HasUserGlobalAdminRightsKey = "HasUserGlobalAdminRights";
        private const string LedgerIdKey = "LedgerId";
        private const string LedgerNameKey = "LedgerName";
        private const string ClientKeyKey = "ClientKey";
        private const string ReferralCodeKey = "ReferralCode";
        private const string AggregationLoggingEnabledKey = "AggregationLoggingEnabled";

        public string UserId
        {
            get { return GetStringSessionValue(UserIdKey); }
            set { SetStringSessionValue(UserIdKey, value); }
        }

        public string UserEmail
        {
            get { return GetStringSessionValue(UserEmailKey); }
            set { SetStringSessionValue(UserEmailKey, value); }
        }

        public bool IsUserAuthorized
        {
            get { return !String.IsNullOrEmpty(UserId); }
        }

        public bool HasUserAffiliateAdminRights
        {
            get { return GetBoolSessionValue(HasUserAffiliateAdminRightsKey); }
            set { SetBoolSessionValue(HasUserAffiliateAdminRightsKey, value); }
        }

        public bool HasUserGlobalAdminRights
        {
            get { return GetBoolSessionValue(HasUserGlobalAdminRightsKey); }
            set { SetBoolSessionValue(HasUserGlobalAdminRightsKey, value); }
        }

        public string LedgerId
        {
            get { return GetStringSessionValue(LedgerIdKey); }
            set { SetStringSessionValue(LedgerIdKey, value); }
        }

        public string LedgerName
        {
            get { return GetStringSessionValue(LedgerNameKey); }
            set { SetStringSessionValue(LedgerNameKey, value); }
        }

        public string ClientKey
        {
            get { return GetStringSessionValue(ClientKeyKey); }
            set { SetStringSessionValue(ClientKeyKey, value); }
        }

        public string ReferralCode
        {
            get { return GetStringSessionValue(ReferralCodeKey); }
            set { SetStringSessionValue(ReferralCodeKey, value); }
        }
        
        public bool AggregationLoggingEnabled
        {
            get { return GetBoolSessionValue(AggregationLoggingEnabledKey); }
            set { SetBoolSessionValue(AggregationLoggingEnabledKey, value); }
        }


        public void Create(UserDocument user)
        {
            UserId = user.Id;
            UserEmail = user.Email;

            HasUserAffiliateAdminRights = user.Permissions.Contains(UserPermissionEnum.AffiliateAdminView);
            HasUserGlobalAdminRights = user.Permissions.Contains(UserPermissionEnum.GlobalAdminView);
            AggregationLoggingEnabled = user.Settings.EnableIntuitLogging;
        }

        public void Logout()
        {
            HttpContext.Current.Session.Abandon();
            UserId = null;
            UserEmail = null;
            LedgerId = null;
        }

        public string GetStringSessionValue(string key)
        {
            var value = HttpContext.Current.Session[key];

            return value == null ? null : value.ToString();
        }

        public void SetStringSessionValue(string key, string value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public bool GetBoolSessionValue(string key)
        {
            var item = HttpContext.Current.Session[key];
            var result = false;
            if (item != null)
            {
                bool.TryParse(item.ToString(), out result);
            }

            return result;
        }

        public void SetBoolSessionValue(string key, bool value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public void RemoveSessionValue(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                HttpContext.Current.Session.Remove(key);
            }
        }

        public void SetSessionValue(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public object GetSessionValue(string key)
        {
            return HttpContext.Current.Session[key];
        }
    }
}