using System;
using System.Collections.Generic;
using FusionChartsMVC.FusionCharts.Charts;

namespace Default.ViewModel.Areas.Finance.TrendsController
{
    public enum TrendModelFilterEnum
    {
        SelectedMonth = 1, 
        PrevMonth = 2, 
        Year = 3,
        AllTime = 4
    }

    public enum ShowFormatEnum
    {
        Spending = 1, 
        Income = 2
    }

    public class TrendCategoryItem
    {
        public string Name { get; set; }

        public long Amount { get; set; }

        public string AccountId { get; set; }

        public bool HasSubCategories { get; set; }
    }

    public class TrendMonthModel
    {
        public string Text { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
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

        public PieChart Chart { get; set; }

        public string SelectedCategoryId { get; set; }

        public string SelectedCategoryName { get; set; }

        public bool All { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public bool LoadSubAccounts { get; set; }
    }
}
