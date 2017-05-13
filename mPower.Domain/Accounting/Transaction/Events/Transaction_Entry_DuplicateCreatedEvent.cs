using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Transaction.Data;

namespace mPower.Domain.Accounting.Transaction.Events
{
    public class Transaction_Entry_DuplicateCreatedEvent : Event
    {
        public string DuplicateId { get; set; }
        public string LedgerId { get; set; }
        public ExpandedEntryData ManualEntry { get; set; }
        public List<ExpandedEntryData> PotentialDuplicates { get; set; }
    }
}