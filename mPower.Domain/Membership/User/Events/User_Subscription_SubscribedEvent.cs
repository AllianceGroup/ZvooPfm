using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_Subscription_SubscribedEvent : Event
    {
        /// <summary>
        /// It's customer id from chargify
        /// </summary>
        public string SubscriptionId { get; set; }

        public int ChargifySubscriptionId { get; set; }

        #region Product Info

        public string ProductName { get; set; }

        public string ProductHandle { get; set; }

        public int ProductPriceInCents { get; set; }

        #endregion

        #region Customer Info

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Organization { get; set; }

        public string UserId { get; set; }

        #endregion

        #region CC Info

        public string FirstNameCC { get; set; }
 
        public string LastNameCC { get; set; }

        public int ExpirationMonth { get; set; }

        public int ExpirationYear { get; set; }

        public string FullNumber { get; set; }

        public string CVV { get; set; }

        public string BillingAddress { get; set; }

        public string BillingCity { get; set; }

        public string BillingCountry { get; set; }

        public string BillingState { get; set; }

        public string BillingZip { get; set; }
            
        #endregion
    }
}
