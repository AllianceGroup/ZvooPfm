using Paralect.Domain;
using mPower.Domain.Membership.Enums;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_UpdateSecurityLevelCommand : Command
    {
        public string UserId { get; set; }

        public SecurityLevelEnum SecurityLevel { get; set; }
    }
}