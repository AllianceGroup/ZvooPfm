using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Transaction.Data;

namespace mPower.Domain.Accounting.Transaction.Events
{
    public class Transaction_DuplicateCreatedEvent : Event
    {
        public ExpandedEntryData BaseTransaction { get; set; }
        public List<ExpandedEntryData> PotentialDuplicates { get; set; }
    }
}