using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Transaction.Data;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_Entry_ChangeAccountCommand : Command
    {
        public string LedgerId { get; set; }
        public string TransactionId { get; set; }
        public string BaseEntryAccountId { get; set; }
        public AccountTypeEnum BaseEntryAccountType { get; set; }
        public bool IsTransactionImported { get; set; }
        public string ReferenceNumber { get; set; }
        public TransactionType TransactionType { get; set; }

        public List<ExpandedEntryData> Entries { get; set; }

        public string PreviousAccountId { get; set; }
        public string NewAccountId { get; set; }
    }
}
