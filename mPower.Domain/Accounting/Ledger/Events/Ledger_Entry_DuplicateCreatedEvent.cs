using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Ledger.Data;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Entry_DuplicateCreatedEvent : Event
    {
        public string DuplicateId { get; set; }
        public string LedgerId { get; set; }
        public ExpandedEntryData ManualEntry { get; set; }
        public List<ExpandedEntryData> PotentialDuplicates { get; set; }
    }
}