using System;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Accounting.Ledger
{
    public class EntryDocument
    {
        public bool Imported { get; set; }

        [BsonId]
        public string Id { get; set; }
        public string LedgerId { get; set; }
        public string TransactionId { get; set; }
        public string AccountId { get; set; }

        public string AccountName { get; set; }
        public AccountTypeEnum AccountType { get; set; }
        public AccountLabelEnum AccountLabel { get; set; }

        public string OffsetAccountName { get; set; }
        public string OffsetAccountId { get; set; }

        public DateTime BookedDate { get; set; }
        public Int64 CreditAmountInCents { get; set; }
        public Int64 DebitAmountInCents { get; set; }

        public Int64 EntryBalance
        {
            get
            {
                return AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(DebitAmountInCents,
                                                                                             CreditAmountInCents,
                                                                                             AccountType);
            }
            set { }
        }

        public string FormattedAmountInDollars { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public string BookedDateString { get; set; }


    }
}
