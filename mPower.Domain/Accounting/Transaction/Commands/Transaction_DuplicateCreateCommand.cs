using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Transaction.Data;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_DuplicateCreateCommand : Command
    {
        public ExpandedEntryData BaseTransaction { get; set; }
        public List<ExpandedEntryData> PotentialDuplicates { get; set; }
    }
}