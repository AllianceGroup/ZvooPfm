using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_GoogleSignupCommand : Command
    {
        public string UserId { get; set; }

        public string ApplicationId { get; set; }

        public string GoogleUserId { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string FormattedName { get; set; }

        public string VerifiedEmail { get; set; }

        public string DisplayName { get; set; }

        public string PreferredUsername { get; set; }

        public string ProfileUrl { get; set; }

        public string Identifier { get; set; }

        public string Email { get; set; }

        public string ReferralCode { get; set; }

        #region Signup Info

        public string SignupUserName { get; set; }

        public string SignupEmail { get; set; }

        public string SignupFirstName { get; set; }

        public string SignupLastName { get; set; }

        #endregion
    }
}
