using System;
using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Transaction.Data;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_CreateCommand : Command
    {
        public AccountTypeEnum? BaseEntryAccountType { get; set; }
        public string BaseEntryAccountId { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserId { get; set; }
        public string LedgerId { get; set; }
        public string TransactionId { get; set; }
        public TransactionType Type { get; set; }
        public List<ExpandedEntryData> Entries { get; set; }
        public bool Imported { get; set; }
        public string ImportedTransactionId { get; set; }
    }
}