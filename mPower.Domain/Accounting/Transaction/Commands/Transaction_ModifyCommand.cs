using System;
using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Transaction.Data;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_ModifyCommand : Command
    {
        public bool Imported { get; set; }
        public string BaseEntryAccountId { get; set; }
        public AccountTypeEnum BaseEntryAccountType { get; set; }

        public String LedgerId { get; set; }
        public String TransactionId { get; set; }
        public TransactionType Type { get; set; }
        public List<ExpandedEntryData> Entries { get; set; }
        public string ReferenceNumber { get; set; }
		public DateTime BookedDate { get; set; }
    }
}