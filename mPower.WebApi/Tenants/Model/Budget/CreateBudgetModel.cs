using System.Collections.Generic;
using Default.ViewModel.Areas.Finance.BudgetController;

namespace mPower.WebApi.Tenants.Model.Budget
{
    public class CreateBudgetModel
    {
        public List<BudgetItemContract> Income { get; set; }

        public List<BudgetItemContract> Expense { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}
