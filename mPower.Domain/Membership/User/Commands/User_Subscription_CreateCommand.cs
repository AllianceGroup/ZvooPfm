using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Subscription_CreateCommand : Command
    {
        public string UserId { get; set; }

        public string SubscriptionId { get; set; }

        public string CreditIdentityId { get; set; }
    }
}
