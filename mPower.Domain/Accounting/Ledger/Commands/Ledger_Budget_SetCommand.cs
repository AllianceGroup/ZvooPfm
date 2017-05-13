using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Ledger.Data;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Budget_SetCommand : Command
    {
        public string LedgerId { get; set; }

        public List<BudgetData> Budgets { get; set; }
    }
}
