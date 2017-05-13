using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_GoalsLinkedAccount_SetEvent : Event
    {
        public string UserId { get; set; }

        public string LedgerId { get; set; }

        public string AccountId { get; set; }
    }
}