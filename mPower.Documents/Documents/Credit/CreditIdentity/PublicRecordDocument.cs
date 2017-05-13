using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class PublicRecordDocument
    {
        [BsonId]
        public string Id { get; set; }

        public DateTime DateFiled { get; set; }
        public DateTime DateVerified { get; set; }
        public DateTime DateExpires { get; set; }

        public string ClassificationDescription { get; set; }
        public string ClassificationSymbol { get; set; }
        public int ClassificationRank { get; set; }
        public string CourtName { get; set; }
        public string DesignatorDescription { get; set; }
        public string Bureau { get; set; }
        public string IndustryCodeDescription { get; set; }
        public string IndustryCodeSymbol { get; set; }
        public int IndustryRank { get; set; }
        public string ReferenceNumber { get; set; }
        public List<string> CustomRemarks { get; set; }
        public string SubscriberCode { get; set; }
        public string Status { get; set; }

        public string Type { get; set; }
        public dynamic Item { get; set; } 
    }
}
