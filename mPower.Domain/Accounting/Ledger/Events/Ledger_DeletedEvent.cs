using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_DeletedEvent : Event
    {
        public String LedgerId { get; set; }
    }
}