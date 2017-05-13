using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_ActivatedEvent : Event
    {
        public string UserId { get; set; }

        public bool IsAdmin { get; set; }
    }
}
