using System;
using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Ledger.Data;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Budget_SetEvent : Event
    {
        public String LedgerId { get; set; }

        public List<BudgetData> Budgets { get; set; }
    }
}