using System;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Data
{
    public class ExpandedEntryData
    {
        public String AccountId { get; set; }
        public string AccountName { get; set; }
        public AccountTypeEnum AccountType { get; set; }
        public AccountLabelEnum AccountLabel { get; set; }

        public string OffsetAccountName { get; set; }
        public string OffsetAccountId { get; set; }


        public DateTime BookedDate { get; set; }
        public Int64 CreditAmountInCents { get; set; }
        public Int64 DebitAmountInCents { get; set; }
        public String Payee { get; set; }
        public String Memo { get; set; }

        public String LedgerId { get; set; }
        public String TransactionId { get; set; }
        public bool TransactionImported { get; set; }
       
    }
}