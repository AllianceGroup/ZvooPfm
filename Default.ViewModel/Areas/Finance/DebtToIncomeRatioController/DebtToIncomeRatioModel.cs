using System.Collections.Generic;
using Default.ViewModel.Areas.Shared;
using FusionChartsMVC.FusionCharts.Charts;

namespace Default.ViewModel.Areas.Finance.DebtToIncomeRatioController
{
    public class DebtToIncomeRatioModel
    {
        public AccountsSidebarModel AccountsSidebar { get; set; }

        public bool IsDebtToIncomeCalculatedBefore { get; set; }

        public decimal MonthlyGrossIncome { get; set; }

        public decimal TotalMonthlyRent { get; set; }

        public decimal TotalMonthlyPitia { get; set; }

        public decimal TotalMonthlyDebt { get; set; }

        public string DebtToIncomeRatio { get; set; }

        public List<ChartItem> LeftChart { get; set; }

        public List<ChartItem> RightChart { get; set; }
    }

}
