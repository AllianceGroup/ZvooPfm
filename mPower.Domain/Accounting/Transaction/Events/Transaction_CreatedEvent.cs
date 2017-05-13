using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Transaction.Data;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Transaction.Events
{
    public class Transaction_CreatedEvent : Event
    {
        public AccountTypeEnum BaseEntryAccountType { get; set; }
        public string BaseEntryAccountId { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserId { get; set; }
        public string LedgerId { get; set; }
        public string TransactionId { get; set; }
        public TransactionType Type { get; set; }
        public List<ExpandedEntryData> Entries { get; set; }
        public string Memo { get; set; }
        public bool Imported { get; set; }
        public string ImportedTransactionId { get; set; }
        [Obsolete]
        public long NewBalanceOfAggregatedAccount { get; set; }
        public bool IsMultipleInsert { get; set; }
    }
}