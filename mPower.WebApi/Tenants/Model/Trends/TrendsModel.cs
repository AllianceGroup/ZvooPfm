using System;
using System.Collections.Generic;
using Default.ViewModel.Areas.Finance.TrendsController;

namespace mPower.WebApi.Tenants.Model.Trends
{
    public enum TrendModelFilterEnum
    {
        ThisMonth = 1,
        PrevMonth = 2,
        SelectedMonth = 3,
        Year = 4,
        AllTime = 5
    }

    public class TrendsModel
    {
        public TrendsModel()
        {
            Categories = new List<TrendCategoryItem>();
        }
        public DateTime Date { get; set; }

        public List<TrendMonthModel> Monthes { get; set; }

        public List<TrendCategoryItem> Categories { get; set; } 

        public Dictionary<int, string> ShowFormats { get; set; } 

        public ShowFormatEnum SelectedShowFormat { get; set; }

        public Dictionary<int, string> TrendFilters { get; set; } 

        public TrendModelFilterEnum SelectedFilter { get; set; }

        public string SelectedCategoryId { get; set; }

        public string SelectedCategoryName { get; set; }

        public bool All { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public bool LoadSubAccounts { get; set; }
    }
}
