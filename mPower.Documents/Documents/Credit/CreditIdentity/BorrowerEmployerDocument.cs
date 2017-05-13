using System;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class BorrowerEmployerDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime DateReported { get; set; }
        public AddressDocument Address { get; set; } 
    }
}
