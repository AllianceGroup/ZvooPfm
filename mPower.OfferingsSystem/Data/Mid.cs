using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Mid: AccessRecord
    {
        [CsvField(Name = "phoneNumber")]
        public string Phone { get; set; }

        [CsvField(Name = "streetLine1")]
        public string StreetLine1 { get; set; }

        [CsvField(Name = "streetLine2")]
        public string StreetLine2 { get; set; }

        [CsvField(Name = "postalCode")]
        public string PostalCode { get; set; }

        [CsvField(Name = "city")]
        public string City { get; set; }

        [CsvField(Name = "state")]
        public string State { get; set; }

        [CsvField(Name = "latitude")]
        public string Latitude { get; set; }

        [CsvField(Name = "longitude")]
        public string Longitude { get; set; }

        [CsvField(Name = "country")]
        public string Country { get; set; }

        [CsvField(Name = "recordStatus")]
        public string Status { get; set; }

        [CsvField(Name = "recordStatusMessage")]
        public string StatusMessage { get; set; }

        [CsvField(Name = "brandIdenti?er")]
        public string BrandId { get; set; }

        [CsvField(Name = "locationIdenti?er")]
        public string LocationId { get; set; }

        [CsvField(Name = "startDate")]
        public string StartDate { get; set; }

        [CsvField(Name = "endDate")]
        public string EndDate { get; set; }

        [CsvField(Name = "midValue")]
        public string Value { get; set; }

        [CsvField(Name = "midType")]
        public string Type { get; set; }

        [CsvField(Name = "processor")]
        public string Processor { get; set; }

        [CsvField(Name = "acquirer")]
        public string Acquirer { get; set; }

        [CsvField(Name = "midAuthFormName")]
        public string AuthFormName { get; set; }

        [CsvField(Name = "cardIdentifier")]
        public string CardId { get; set; }

        [CsvField(Name = "lastFour")]
        public string LastFour { get; set; }

        [CsvField(Name = "ttxDatetime")]
        public string TtxDatetime { get; set; }

        [CsvField(Name = "ttxAmount")]
        public string TtxAmount { get; set; }
    }
}