using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_AddTwitterAccountCommand : Command
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
