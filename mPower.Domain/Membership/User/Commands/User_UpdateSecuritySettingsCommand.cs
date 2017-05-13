using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_UpdateSecuritySettingsCommand: Command
    {
        public string UserId { get; set; }

        public bool EnableAdminAccess { get; set; }

        public bool EnableAggregationLogging { get; set; }
    }
}