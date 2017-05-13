using System;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class AlertDocument
    {
        [BsonId]
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public AlertTypeEnum Type { get; set; }

        public string Message { get; set; }
    }
}
