using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Account_ArchivedEvent : Event
    {
        public String LedgerId { get; set; }
        public String AccountId { get; set; }
        public String Reason { get; set; }
    }
}
