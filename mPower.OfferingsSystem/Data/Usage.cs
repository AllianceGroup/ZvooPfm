using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Usage: AccessRecord
    {
        [CsvField(Name = "organizationCustomerIdentifier")]
        public string OrganizationCustomerId { get; set; }

        [CsvField(Name = "programCustomerIdentifier")]
        public string ProgrammCustomerId { get; set; }

        [CsvField(Name = "memberCustomerIdentifier")]
        public string MemberCustomerId { get; set; }

        [CsvField(Name = "eventDatetime")]
        public string EventDatetime { get; set; }

        [CsvField(Name = "brandIdentifier")]
        public string BrandId { get; set; }

        [CsvField(Name = "locationIdentifier")]
        public string LocationId { get; set; }

        [CsvField(Name = "offerIdentifier")]
        public string OfferId { get; set; }

        [CsvField(Name = "offerDataIdentifier")]
        public string OfferDataId { get; set; }

        [CsvField(Name = "publicationChannel")]
        public string PublicationChannel { get; set; }

        [CsvField(Name = "action")]
        public string Action { get; set; }

        [CsvField(Name = "description")]
        public string Description { get; set; }      
    }
}