using Paralect.Domain;
using mPower.Domain.Membership.Enums;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_RemovePermissionCommand : Command
    {
        public string UserId { get; set; }

        public UserPermissionEnum Permission { get; set; }
    }
}
