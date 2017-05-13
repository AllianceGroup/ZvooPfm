using System;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class InquiryDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Bureau { get; set; }
        public string IndustryCodeAbbreviation { get; set; }
        public string IndustryCodeDescription { get; set; }
        public int IndustryCodeRank { get; set; }
        public DateTime InquiryDate { get; set; }
        public string InquiryType { get; set; }
        public string SubscriberName { get; set; }
        public string SubscriberNumber { get; set; } 
    }
}
