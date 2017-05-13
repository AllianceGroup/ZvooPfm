using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Transaction.Data;

namespace mPower.Domain.Accounting.Transaction.Messages
{
    public class Entries_AddedMessage : Event
    {
        public string UserId { get; set; }

        public string LedgerId { get; set; }

        public string TransactionId { get; set; }

        public List<ExpandedEntryData> Entries { get; set; }
    }
}