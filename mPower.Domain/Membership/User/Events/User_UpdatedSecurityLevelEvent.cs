using Paralect.Domain;
using mPower.Domain.Membership.Enums;

namespace mPower.Domain.Membership.User.Events
{
    public class User_UpdatedSecurityLevelEvent : Event
    {
        public string UserId { get; set; }

        public SecurityLevelEnum SecurityLevel { get; set; }
    }
}