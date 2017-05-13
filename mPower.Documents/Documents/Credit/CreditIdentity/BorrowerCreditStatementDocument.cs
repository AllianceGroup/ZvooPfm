using System;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class BorrowerCreditStatementDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Bureau { get; set; }
        public DateTime InquiryDate { get; set; }
        public string Statement { get; set; }
        public string StatementTypeAbbreviation { get; set; }
        public string StatementTypeDescription { get; set; }
        public string StatementTypeSymbol { get; set; }
        public int StatementTypeRank { get; set; } 
    }
}
