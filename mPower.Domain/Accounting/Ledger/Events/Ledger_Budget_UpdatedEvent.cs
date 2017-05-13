using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Budget_UpdatedEvent : Event
    {
        public String LedgerId { get; set; }

        public string BudgetId { get; set; }

        public long Amount { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}