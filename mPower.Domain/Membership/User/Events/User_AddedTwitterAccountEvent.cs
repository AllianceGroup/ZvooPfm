using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_AddedTwitterAccountEvent : Event
    {
        public string UserId { get; set; }

        public string PhotoUrl { get; set; }

        public string FormattedName { get; set; }

        public string DisplayName { get; set; }

        public string PreferredUsername { get; set; }

        public string ProfileUrl { get; set; }

        public string Identifier { get; set; }
    }
}
