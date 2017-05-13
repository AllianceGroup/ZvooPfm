using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_Email_RemovedEvent : Event
    {
        public string UserId { get; set; }

        public string Email { get; set; } 
    }
}