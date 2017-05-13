using System.Linq;

namespace Default.Areas.Finance.Controllers
{
    public class SavingsListModel
    {
        public string PeriodTitle { get; set; }
        public IOrderedEnumerable<AccountOffersViewModel> Accounts { get; set; }
        public decimal TotalSpent { get; set; }
        public double PotentialSavingsPerc { get; set; }
        public decimal PotentialSavingsInDollars { get; set; }

        public StatisticFilterModel Filter { get; set; }

        public string ForecastMonthsCountFotmattedString { get { return Filter.GetForecastMonthsCount() > 1 ? string.Format("{0} monthes",Filter.GetForecastMonthsCount()) : "month"; }}
    }

    public class SavingsServiceViewModel
    {
        public string PotentialSavingsInDollars { get; set; }
    }

}