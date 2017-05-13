using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Accounting.Ledger
{
    public class TransactionDuplicateDocument
    {
        [BsonId]
        public string BaseTransactionId { get; set; }
        public string LedgerId { get; set; }
        public TransactionDuplicateDataDocument BaseTransaction { get; set; }
        public List<TransactionDuplicateDataDocument> PotentialDuplicates { get; set; } 
        
    }

    public class TransactionDuplicateDataDocument
    {
        public string OffsetAccountId { get; set; }
        public string Payee { get; set; }
        public string TransactionId { get; set; }
        public string FormattedAmount { get; set; }
        public string OffsetAccountName { get; set; }
        public DateTime Date { get; set; }
        public long AmountInCents { get; set; }
        public string AccountName { get; set; }
        public string Memo { get; set; }
    }
}
