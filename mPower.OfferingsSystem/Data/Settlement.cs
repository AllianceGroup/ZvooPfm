using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Settlement: AccessRecord
    {
        [CsvField(Name = "organizationCustomerIdentifier")]
        public string OrganizationCustomerId { get; set; }

        [CsvField(Name = "programCustomerIdentifier")]
        public string ProgrammCustomerId { get; set; }

        [CsvField(Name = "settlementIdentifier")]
        public string SettlementId { get; set; }

        [CsvField(Name = "settlementStartDate")]
        public string StartDate { get; set; }

        [CsvField(Name = "settlementEndDate")]
        public string EndDate { get; set; }

        [CsvField(Name = "totalRewards")]
        public string TotalRewards { get; set; }

        [CsvField(Name = "totalFunded")]
        public string TotalFunded { get; set; }
    }
}