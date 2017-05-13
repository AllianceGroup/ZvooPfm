using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Transaction.Data;

namespace mPower.Documents
{
    public class TransactionModifyDto
    {
        public bool Imported { get; set; }
        public string BaseEntryAccountId { get; set; }

        public String LedgerId { get; set; }
        public String TransactionId { get; set; }
        public TransactionType Type { get; set; }
        public List<EntryData> Entries { get; set; }
        public string ReferenceNumber { get; set; }
    }
}