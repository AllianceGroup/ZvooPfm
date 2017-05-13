using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Brand: AccessRecord
    {
        [CsvField(Name = "brandName")]
        public string Name { get; set; }

        [CsvField(Name = "brandIdentifier")]
        public string BrandId { get; set; }

        [CsvField(Name = "brandDescription")]
        public string Description { get; set; }

        [CsvField(Name = "brandLogoName")]
        public string LogoName { get; set; }
    }
}