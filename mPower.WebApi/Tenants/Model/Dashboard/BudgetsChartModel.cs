using System.Collections.Generic;
using Default.ViewModel.Areas.Finance.DebtToIncomeRatioController;

namespace mPower.WebApi.Tenants.Model.Dashboard
{
    public class BudgetsChartModel
    {
        public List<ChartItem> Spent { get; set; }

        public List<ChartItem> Budget { get; set; }
    }
}
