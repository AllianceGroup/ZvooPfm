using Default.Factories.ViewModels;
using FusionChartsMVC.FusionCharts.Charts;

namespace Default.Areas.Administration.Models
{
    public class AnalyticsDashboardModel
    {
        public string ImageSrc { get; set; }

        public Chart Chart { get; set; }

        public AnalyticsModel TotalMoneyManaged { get; set; }
        public AnalyticsModel TotalUserDebt { get; set; }
        public AnalyticsModel AvgUserAnnualIncome { get; set; }
        public AnalyticsModel AvailableCash { get; set; }
        public AnalyticsModel AvailableCredit { get; set; }

        public StatisticTypeEnum TotalDebtType { get; set; }
        public StatisticTypeEnum AvailableCashType { get; set; }

        public AnalyticsDashboardModel()
        {
            TotalMoneyManaged = new AnalyticsModel();
            TotalUserDebt = new AnalyticsModel();
            AvailableCash = new AnalyticsModel();
            AvailableCredit = new AnalyticsModel();
            AvgUserAnnualIncome = new AnalyticsModel();

            TotalDebtType = AvailableCashType = StatisticTypeEnum.Average;
        }
    }

    public class AnalyticsModel
    {
        public long Amount { get; set; }

        public int Past30DaysGrossing { get; set; }

        public int Past60DaysGrossing { get; set; }

        public int Past90DaysGrossing { get; set; }
    }
}