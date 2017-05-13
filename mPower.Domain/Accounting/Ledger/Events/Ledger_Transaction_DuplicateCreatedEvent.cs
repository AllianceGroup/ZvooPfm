using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Ledger.Data;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Transaction_DuplicateCreatedEvent : Event
    {
        public ExpandedEntryData BaseTransaction { get; set; }
        public List<ExpandedEntryData> PotentialDuplicates { get; set; }
    }
}