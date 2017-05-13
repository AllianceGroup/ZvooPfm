using System;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class BorrowerSocialDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Bureau { get; set; }
        public DateTime InquiryDate { get; set; }
        public string SocialSecurityNumber { get; set; } 
    }
}
