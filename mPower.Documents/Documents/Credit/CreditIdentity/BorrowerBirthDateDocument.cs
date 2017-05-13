using System;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class BorrowerBirthDateDocument
    {
        [BsonId]
        public string Id { get; set; }

        public decimal Age { get; set; }
        public DateTime BirthDate { get; set; }
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; } 
    }
}
