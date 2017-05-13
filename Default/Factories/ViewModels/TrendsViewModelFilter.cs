using System;
using Default.ViewModel.Areas.Finance.TrendsController;
using mPower.Framework.Utils;

namespace Default.Models
{
    public class TrendsViewModelFilter
    {
        public const string CategoryAcccountIdUrlKey = "CategoryAccountId";

        public TrendModelFilterEnum Filter { get; set; }
        public ShowFormatEnum ShowFormat{ get; set; }
        public int Month{ get; set; }
        public int Year{ get; set; }
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
                    case TrendModelFilterEnum.SelectedMonth:
                        return  DateUtil.GetEndOfMonth(Month, Year).ToShortDateString();
                }
                return String.Empty;
            }
        }

        public string FilterByAccountUrl { get; set; }

        public TrendsViewModelFilter()
        {
            TakeCategories = 10;
            Filter = TrendModelFilterEnum.SelectedMonth;
            ShowFormat = ShowFormatEnum.Spending;
            Month = 0;
            Year = 0;
        }
    }
}