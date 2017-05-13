using Paralect.Domain;
using mPower.Domain.Membership.Enums;

namespace mPower.Domain.Membership.User.Events
{
    public class User_PermissionAddedEvent : Event
    {
        public string UserId { get; set; }

        public UserPermissionEnum Permission { get; set; }
    }
}
