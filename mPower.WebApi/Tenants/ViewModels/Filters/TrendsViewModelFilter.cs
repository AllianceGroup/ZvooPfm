using System;
using Default.ViewModel.Areas.Finance.TrendsController;
using mPower.Framework.Utils;
using TrendModelFilterEnum = mPower.WebApi.Tenants.Model.Trends.TrendModelFilterEnum;

namespace mPower.WebApi.Tenants.ViewModels.Filters
{
    public class TrendsViewModelFilter
    {
        public const string CategoryAcccountIdUrlKey = "CategoryAccountId";

        public TrendModelFilterEnum Filter { get; set; }
        public ShowFormatEnum ShowFormat { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int TakeCategories { get; set; }
        public string AccountId { get; set; }
        public string LedgerId { get; set; }
        public bool All { get; set; }

        public string From
        {
            get
            {
                switch (Filter)
                {
                    case TrendModelFilterEnum.AllTime:
                        return string.Empty;
                    case TrendModelFilterEnum.Year:
                        return DateUtil.GetStartOfCurrentYear().ToShortDateString();
                    case TrendModelFilterEnum.PrevMonth:
                        return DateUtil.GetStartOfLastMonth().ToShortDateString();
                    case TrendModelFilterEnum.ThisMonth:
                        return DateUtil.GetStartOfCurrentMonth().ToShortDateString();
                    case TrendModelFilterEnum.SelectedMonth:
                        return DateUtil.GetStartOfMonth(Month, Year).ToShortDateString();
                }
                return String.Empty;
            }
        }

        public string To
        {
            get
            {
                switch (Filter)
                {
                    case TrendModelFilterEnum.AllTime:
                        return string.Empty;
                    case TrendModelFilterEnum.Year:
                        return DateUtil.GetEndOfCurrentYear().ToShortDateString();
                    case TrendModelFilterEnum.PrevMonth:
                        return DateUtil.GetEndOfLastMonth().ToShortDateString();
                    case TrendModelFilterEnum.ThisMonth:
                        return DateUtil.GetEndOfCurrentMonth().ToShortDateString();
                    case TrendModelFilterEnum.SelectedMonth:
                        return DateUtil.GetEndOfMonth(Month, Year).ToShortDateString();
                }
                return String.Empty;
            }
        }

        public string FilterByAccountUrl { get; set; }

        public TrendsViewModelFilter()
        {
            TakeCategories = 10;
            Filter = TrendModelFilterEnum.ThisMonth;
            ShowFormat = ShowFormatEnum.Spending;
            Month = 0;
            Year = 0;
        }
    }
}
