using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.TempDocuments.Server.Documents
{
    public class OfferGroupDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public List<OfferDocument> Offers { get; set; }

        public string Title { get; set; } 

        public string Merchant { get; set; } 

        public OfferGroupDocument()
        {
            Offers= new List<OfferDocument>();
        }
    }
}