using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Merchant : AccessRecord
    {
        [CsvField(Name = "brandIdentifier")]
        public string BrandId { get; set; }

        [CsvField(Name = "locationIdentifier")]
        public string LocationId { get; set; }

        [CsvField(Name = "locationName")]
        public string LocationName { get; set; }

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

        [CsvField(Ignore = true)]
        public double? Latitude
        {
            get
            {
                double value;
                if (double.TryParse(LatitudeString, out value))
                {
                    return value;
                }
                return null;
            }
        }

        [CsvField(Name = "latitude")]
        public string LatitudeString { get; set; }

        [CsvField(Ignore = true)]
        public double? Longitude
        {
            get
            {
                double value;
                if (double.TryParse(LongitudeString, out value))
                {
                    return value;
                }
                return null;
            }
        }

        [CsvField(Name = "longitude")]
        public string LongitudeString { get; set; }

        [CsvField(Name = "country")]
        public string Country { get; set; }

        [CsvField(Name = "locationUrl")]
        public string LocationUrl { get; set; }

        [CsvField(Name = "locationLogoName")]
        public string LocationLogo { get; set; }

        [CsvField(Name = "locationPhotoNames")]
        public string LocationPhotos { get; set; }
    
        [CsvField(Name = "keywords")]
        public string Keywords { get; set; }

        [CsvField(Name = "locationDescription")]
        public string LocationDescription { get; set; }

        [CsvField(Name = "serviceArea")]
        public string ServiceArea { get; set; }
    }
}