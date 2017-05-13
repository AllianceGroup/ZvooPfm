using System;

namespace mPower.Domain.Accounting.Ledger.Data
{
    public class BudgetExceededData
    {
        public string UserId { get; set; }
        public string BudgetId { get; set; }
        public string LedgerId { get; set; }
        public string AccountName { get; set; }
        public int Month { get; set; }
        public long BudgetAmount { get; set; }
        public long SpentAmountWithSubAccounts { get; set; }
        public DateTime Date { get; set; }
    }
}