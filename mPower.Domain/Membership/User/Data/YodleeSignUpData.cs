using System;

namespace mPower.Domain.Membership.User.Data
{
    public class YodleeSignUpData
    {
        public string Password { get; set; }
        public long LastLoginTime { get; set; }
        public long LoginCount { get; set; }
        public bool PasswordRecovered { get; set; }
        public string EmailAddress { get; set; }
        public string LoginName { get; set; }
        public string UserType { get; set; }
        public DateTime PasswordChangedOn { get; set; }
        public int? PasswordExpiryNotificationDays { get; set; }
        public int? PasswordExpiryDays { get; set; }

    }
}
