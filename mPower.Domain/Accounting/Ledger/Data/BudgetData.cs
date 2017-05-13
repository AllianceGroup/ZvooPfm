using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Data
{
    public class BudgetData
    {
        public BudgetData()
        {
            SubBudgets = new List<BudgetData>();
        }

        public string Id { get; set; }

        public string AccountId { get; set; }

        public string ParentId { get; set; }

        public string AccountName { get; set; }

        public AccountTypeEnum AccountType { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public long SpentAmount { get; set; }

        public long BudgetAmount { get; set; }

        public List<BudgetData> SubBudgets { get; set; }
    }
}
