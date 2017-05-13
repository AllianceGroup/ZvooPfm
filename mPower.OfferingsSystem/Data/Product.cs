using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Product: AccessRecord
    {
        [CsvField(Name = "organizationCustomerIdentifer")]
        public string OrganizationCustomerId { get; set; }

        [CsvField(Name = "programCustomerIdentifer")]
        public string ProgrammCustomerId { get; set; }

        [CsvField(Name = "productIdentifer")]
        public string ProductId { get; set; }

        [CsvField(Name = "productName")]
        public string Name { get; set; }

        [CsvField(Name = "productDescription")]
        public string Description { get; set; }

        [CsvField(Name = "productStartDate")]
        public string StartDate { get; set; }

        [CsvField(Name = "productEndDate")]
        public string EndDate { get; set; }
    }
}