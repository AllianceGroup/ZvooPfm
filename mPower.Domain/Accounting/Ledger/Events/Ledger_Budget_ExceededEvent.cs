using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Budget_ExceededEvent : Event
    {
        public string UserId { get; set; }
        public string BudgetId { get; set; }
        public string LedgerId { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public int Month { get; set; }
        public long BudgetAmount { get; set; }
        public long SpentAmountWithSubAccounts { get; set; }
        public DateTime Date { get; set; }
    }
}