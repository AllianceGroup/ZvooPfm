using Paralect.Domain;
using mPower.Domain.Membership.Enums;

namespace mPower.Domain.Membership.User.Events
{
    public class User_PermissionRemovedEvent : Event
    {
        public string UserId { get; set; }

        public UserPermissionEnum Permission { get; set; }
    }
}
