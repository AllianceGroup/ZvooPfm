using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class BorrowerTelephoneDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string AreaCode { get; set; }
        public string Extension { get; set; }
        public string Number { get; set; } 
    }
}
