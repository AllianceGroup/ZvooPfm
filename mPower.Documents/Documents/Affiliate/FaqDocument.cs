using System;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Affiliate
{
   public class FaqDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Html { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationDate { get; set; }       
    }
}
