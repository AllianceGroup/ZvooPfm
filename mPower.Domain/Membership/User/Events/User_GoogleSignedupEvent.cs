using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_GoogleSignedupEvent : Event
    {
        public string UserId { get; set; }

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
    }
}
