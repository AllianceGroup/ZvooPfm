using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class BorrowerCreditScoreDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Bureau { get; set; }
        public DateTime InquiryDate { get; set; }
        public decimal Score { get; set; }
        public List<BorrowerCreditScoreFactorDocument> CreditScoreFactors { get; set; }  
    }
}
