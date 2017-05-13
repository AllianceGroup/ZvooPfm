using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Subscription_DeleteCommand : Command
    {
        public string UserId { get; set; }

        /// <summary>
        /// Chargify Customer Id
        /// </summary>
        public string SubscriptionId { get; set; }

        public string CancelMessage { get; set; }

        public string CreditIdentityId { get; set; }
    }
}
