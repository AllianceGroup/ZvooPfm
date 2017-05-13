using System.Collections.Generic;
using Default.ViewModel.Areas.Finance.DebtToIncomeRatioController;
using Default.ViewModel.Areas.Finance.DebtToolsController;
using mPower.WebApi.Tenants.Model.Dashboard;

namespace mPower.WebApi.Tenants.ViewModels.Dashboard
{
    public class ChartsViewModel
    {
        public List<ChartItem> AccountsChart { get; set; } 

        public BudgetsChartModel BudgetsChart { get; set; }

        public List<ChartItem> LeftChart { get; set; }

        public List<ChartItem> RightChart { get; set; }

        public string DebtToIncomeRatio { get; set; }

        public List<dynamic> MortageAccelerator { get; set; }

        public long TotalSavingsInCents { get; set; }
    }
}
