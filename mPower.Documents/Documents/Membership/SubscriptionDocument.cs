using MongoDB.Bson.Serialization.Attributes;
using mPower.Documents.Enums;

namespace mPower.Documents.Documents.Membership
{
    public class SubscriptionDocument
    {
        /// <summary>
        /// Customer id from chargify
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        public int ChargifySubscriptionId { get; set; }

        public SubscriptionStatusEnum Status { get; set; }

        public string StatusName => Status.ToString();

        /// <summary>
        /// When user setup additional credit identity we initialazing this field.
        /// </summary>
        public string CreditIdentityId { get; set; }

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

        public SubscriptionDocument()
        {
            Status = SubscriptionStatusEnum.None;
        }
    }
}
