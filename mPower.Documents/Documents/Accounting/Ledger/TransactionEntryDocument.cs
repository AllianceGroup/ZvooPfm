using System;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Accounting.Ledger
{
    public class TransactionEntryDocument
    {
        public string AccountId { get; set; }
        public AccountLabelEnum AccountLabel { get; set; }
        public DateTime BookedDate { get; set; }
        public Int64 CreditAmountInCents { get; set; }
        public Int64 DebitAmountInCents { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public string BookedDateString { get; set; }
        
    }
}
