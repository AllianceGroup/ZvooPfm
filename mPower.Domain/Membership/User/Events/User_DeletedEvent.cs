using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_DeletedEvent : Event
    {
        public string UserId { get; set; }
    }
}
