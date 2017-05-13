using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_Subscription_DeletedEvent : Event
    {
        public string UserId { get; set; }

        public string SubscriptionId { get; set; }

        public string CreditIdentityId { get; set; }

        public string CancelMessage { get; set; }
    }
}
