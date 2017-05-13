using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.Areas.Shared;
using FusionChartsMVC.FusionCharts.Charts;

namespace Default.ViewModel.Areas.Finance.DashboardController
{
    public class DashboardViewModel
    {
        public AccountsSidebarModel AccountSideBar { get; set; }

        public List<DashboardBudgetModel> Budgets { get; set; }

        public List<DashboardAlertModel> Alerts { get; set; }

        public long TotalIncome { get; set; }

        public long TotalExpense { get; set; }

		public string CreditScore { get; set; }

		public string CreditScoreDate { get; set; }

		public string CreditGrade { get; set; }

        public DashboardChartsModel Charts { get; set; }

        public DashboardViewModel()
        {
            const string notAvailable = "n/a";
            CreditScore = notAvailable;
            CreditScoreDate = notAvailable;
            CreditGrade = notAvailable;
        }
    }
}
