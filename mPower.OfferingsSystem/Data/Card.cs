using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Card: AccessRecord
    {
        [CsvField(Name = "organizationCustomerIdentifier")]
        public string OrganizationCustomerId { get; set; }

        [CsvField(Name = "programCustomerIdentifier")]
        public string ProgrammCustomerId { get; set; }

        [CsvField(Name = "memberCustomerIdentifier")]
        public string MemberCustomerId { get; set; }

        [CsvField(Name = "cardStatus")]
        public string Status { get; set; }

        [CsvField(Name = "cardIdentifier")]
        public string CardId { get; set; }

        [CsvField(Name = "cardBrand")]
        public string Brand { get; set; }

        [CsvField(Name = "cardType")]
        public string Type { get; set; }

        [CsvField(Name = "nameOnCard")]
        public string Name { get; set; }

        [CsvField(Name = "lastFour")]
        public string LastFour { get; set; }

        [CsvField(Name = "expirationDate")]
        public string ExpirationDate { get; set; }
    }
}