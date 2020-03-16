using Paralect.Domain;
using System;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Account_DateLastAggregated_ChangedEvent : Event
    {
        public string LedgerId { get; set; }

        public string AccountId { get; set; }

        public DateTime DateLastAggregated { get; set; }
    }
}