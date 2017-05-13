using System;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class BorrowerCreditStatementData
    {
        public string Bureau { get; set; }
        public DateTime InquiryDate { get; set; }
        public string Statement { get; set; }
        public string StatementTypeAbbreviation { get; set; }
        public string StatementTypeDescription { get; set; }
        public string StatementTypeSymbol { get; set; }
        public int StatementTypeRank { get; set; } 
    }
}