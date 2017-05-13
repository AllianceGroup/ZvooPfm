using System.Collections.Generic;
using Default.Areas.Administration.Models;
using Default.Factories.ViewModels;
using Default.ViewModel.Areas.Finance.DebtToIncomeRatioController;

namespace mPower.WebApi.Tenants.ViewModels.AffiliateAdmin
{
    public class AnalyticsDashboardViewModel
    {
        public List<ChartItem> Chart { get; set; }

        public AnalyticsModel TotalMoneyManaged { get; set; }
        public AnalyticsModel TotalUserDebt { get; set; }
        public AnalyticsModel AvgUserAnnualIncome { get; set; }
        public AnalyticsModel AvailableCash { get; set; }
        public AnalyticsModel AvailableCredit { get; set; }

        public Statistic Statistic { get; set; }
    }

    public class Statistic
    {
        public StatisticTypeEnum TotalDebtType { get; set; }
        public StatisticTypeEnum AvailableCashType { get; set; }
    }
}
