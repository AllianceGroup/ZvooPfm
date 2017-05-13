using System;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_AddYodleeAccountCommand : Command
    {
        
        public string UserId { get; set; }
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