namespace mPower.Domain.Membership.User.Messages
{
    public class User_Subscription_PaymentFailedMessage
    {
        public string SubscriptionId { get; set; }

        public string UserId { get; set; }
    }
}
