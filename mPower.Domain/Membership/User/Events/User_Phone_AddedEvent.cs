using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_Phone_AddedEvent : Event
    {
        public string UserId { get; set; }

        public string Phone { get; set; } 
    }
}