using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class AddressDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string HouseNumber { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public string Unit { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string County { get; set; }
        public string Country { get; set; }  
    }
}
