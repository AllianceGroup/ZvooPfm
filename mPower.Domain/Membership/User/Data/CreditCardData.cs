namespace mPower.Domain.Membership.User.Data
{
    public class CreditCardData
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int ExpirationMonth { get; set; }

        public int ExpirationYear { get; set; }

        public string FullNumber { get; set; }

        public string CVV { get; set; }

        public string BillingAddress { get; set; }

        public string BillingCity { get; set; }

        public string BillingCountry { get; set; }

        public string BillingState { get; set; }

        public string BillingZip { get; set; }
    }
}
