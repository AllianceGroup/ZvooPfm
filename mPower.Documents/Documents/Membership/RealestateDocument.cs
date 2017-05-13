using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Membership
{
    public class RealestateDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public long AmountInCents { get; set; }

        public bool IsIncludedInWorth { get; set; }
    }
}