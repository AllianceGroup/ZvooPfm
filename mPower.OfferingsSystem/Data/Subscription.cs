using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Subscription : AccessRecord
    {
        [CsvField(Name = "offerIdentifier")]
        public string OfferId { get; set; }

        [CsvField(Name = "subscriptionIdentifiers")]
        public string SubscriptionIdentifiers { get; set; }
    }
}