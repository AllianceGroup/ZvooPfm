using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Transaction.Data
{
    public class TransactionData
    {
        public string BaseEntryAccountId { get; set; }

        public AccountTypeEnum BaseEntryAccountType { get; set; }

        public string Id { get; set; }

        public string LedgerId { get; set; }

        public TransactionType Type { get; set; }

        public List<EntryData> Entries { get; set; }

        public string Memo { get; set; }

        public DateTime BookedDate { get; set; }

        public bool Imported { get; set; }

        public string ReferenceNumber { get; set; }
    }
}