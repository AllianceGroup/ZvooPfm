using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    [Obsolete("WE ARE GOING TO MANAGE IsUpdating through the AGGREGATION STATUS. See Ledger AccountDocument")]
    public class Ledger_Account_IsUpdating_ChangedEvent : Event
    {
        public string ContentServiceItemId { get; set; }
        public string LedgerId { get; set; }
        public string AccountId { get; set; }
        public bool IsUpdating { get; set; }
    }
}