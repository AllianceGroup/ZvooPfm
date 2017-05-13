using System;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class CreditScoreDocument
    {
        [BsonId]
        public string Id { get; set; }

        public decimal Score { get; set; }

        public DateTime ScoreDate { get; set; }
    }
}
