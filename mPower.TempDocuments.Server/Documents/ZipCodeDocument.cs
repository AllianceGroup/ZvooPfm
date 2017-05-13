using MongoDB.Bson.Serialization.Attributes;
using mPower.Framework.Geo;

namespace mPower.TempDocuments.Server.Documents
{
    public class ZipCodeDocument
    {
        [BsonId]
        public string Id { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public Location Location { get; set; }
        public int TimeZone { get; set; }
    }
}