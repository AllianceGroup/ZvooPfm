using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_Realestate_DeletedEvent : Event
    {
        public string Id { get; set; }

        public string UserId { get; set; }
    }
}