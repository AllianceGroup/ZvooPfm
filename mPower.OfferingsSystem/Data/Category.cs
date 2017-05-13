using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Category : AccessRecord
    {
        [CsvField(Name = "categoryName")]
        public string Name { get; set; }

        [CsvField(Name = "categoryIdentifier")]
        public string CategoryId { get; set; }

        [CsvField(Name = "categoryDescription")]
        public string Description { get; set; }

        [CsvField(Name = "categoryLogoName")]
        public string LogoName { get; set; }
    }
}