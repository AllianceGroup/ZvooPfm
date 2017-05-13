using Default.ViewModel.Areas.Finance.DebtToIncomeRatioController;
using Default.ViewModel.Areas.Finance.MortgageAcceleration;
using FusionChartsMVC.FusionCharts.Charts;

namespace Default.ViewModel.Areas.Finance.DashboardController
{
    public class DashboardChartsModel
    {
        public PieChart AccountsChart { get; set; }

        public MultiSeriesChart BudgetsChart { get; set; }

        public DebtToIncomeRatioModel DebtToIncomeRatio { get; set; }

        public MortgageProgramModel MortgageProgram { get; set; }

        public DashboardChartsModel()
        {
            DebtToIncomeRatio = new DebtToIncomeRatioModel();
            MortgageProgram = new MortgageProgramModel();
        }
    }
}