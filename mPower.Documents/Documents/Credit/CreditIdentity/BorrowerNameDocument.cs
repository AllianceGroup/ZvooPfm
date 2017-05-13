using System;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class BorrowerNameDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Prefix { get; set; }
        public string Description { get; set; }
        public DateTime InquiryDate { get; set; }
        public string Suffix { get; set; }
        public string Reference { get; set; }
        public string Bureau { get; set; }  
    }
}
