using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_DeactivatedEvent : Event
    {
        public string UserId { get; set; }

        public bool IsAdmin { get; set; }
    }
}
