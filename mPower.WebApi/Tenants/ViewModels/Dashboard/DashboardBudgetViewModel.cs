using System.Collections.Generic;
using mPower.WebApi.Tenants.Model.Dashboard;

namespace mPower.WebApi.Tenants.ViewModels.Dashboard
{
    public class DashboardBudgetViewModel
    {
        public List<DashboardBudgetModel> Budgets { get; set; }

        public string TotalIncome { get; set; }

        public string TotalExpense { get; set; }

        public string RemainingTotal { get; set; }
    }
}
