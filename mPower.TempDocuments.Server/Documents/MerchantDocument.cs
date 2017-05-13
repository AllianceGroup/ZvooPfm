using MongoDB.Bson.Serialization.Attributes;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.DocumentServices;

namespace mPower.TempDocuments.Server.Documents
{
    public class MerchantDocument 
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Country { get; set; }

        public string LocationUrl { get; set; }

        public string LocationLogo { get; set; }

        public string LocationPhotos { get; set; }

        public string Keywords { get; set; }

        public string LocationDescription { get; set; }

        public string ServiceArea { get; set; }

        public string BrandName { get; set; }

        public string BrandLogo { get; set; }
    }
}