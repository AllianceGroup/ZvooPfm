using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mPower.Documents.Documents.Membership;

namespace Default.ViewModel.Areas.Administration
{
    public class UserExtendedInfoModel
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string AuthToken { get; set; }

        public string MobileAccessToken { get; set; }

        public string ResetPasswordToken { get; set; }

        public string PasswordQuestion { get; set; }

        public string PasswordAnswer { get; set; }

        public string SecurityLevel { get; set; }

        public string IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime LastPasswordChangedDate { get; set; }

        public string Permissions { get; set; }

        public string ApplicationId { get; set; }

        public YodleeUserInfoDocument YodleeUserInfo { get; set; }

        public string IsEnrolled { get; set; }

        public string ZipCode { get; set; }

        public string Phones { get; set; }

        public string ReferralCode { get; set; }
    }
}
