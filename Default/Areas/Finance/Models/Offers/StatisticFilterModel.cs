using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using mPower.Documents.Documents.Membership;

namespace Default.Areas.Finance.Controllers
{
    public class StatisticFilterModel
    {
       
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public PeriodFilterTypeEnum PeriodFilterType { get; set; }

        public int GetForecastMonthsCount()
        {
            switch (PeriodFilterType)
            {
                case PeriodFilterTypeEnum.Last30Days:
                    return 1;
                case PeriodFilterTypeEnum.Last60Days:
                    return 2;
                case PeriodFilterTypeEnum.Last90Days:
                    return 3;
                default:
                    return 1;
            }
        }

        private IEnumerable<SelectListItem> _periodFilterTypeSelectList;
        public IEnumerable<SelectListItem> PeriodFilterTypeSelectList
        {
            get { return _periodFilterTypeSelectList ?? (_periodFilterTypeSelectList = new SelectList(Enum.GetNames(typeof(PeriodFilterTypeEnum)))); }
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public IEnumerable<string> GetPayees(UserStatisticDocument stat)
        {
            return stat.MerhchantsSpentInCents.Where(x => GetSpentAmount(x.Value) > 0).Select(x => x.Key);
        }

        public long GetSpentAmount(SpentStatiscticData data)
        {
            switch (PeriodFilterType)
            {
                case PeriodFilterTypeEnum.None:
                    return data.Total;
                case PeriodFilterTypeEnum.Last30Days:
                    return GetForLastDays(30, data);
                case PeriodFilterTypeEnum.Last60Days:
                    return GetForLastDays(60, data);
                case PeriodFilterTypeEnum.Last90Days:
                    return GetForLastDays(90, data);
                case PeriodFilterTypeEnum.CustomRange:
                    return data.GetSpentInPeriod(StartDate.Value, EndDate.Value);                 
                case PeriodFilterTypeEnum.CustomMonthly:
                    return data.GetSpentForMonths(new MonthYear(Month.Value, Year.Value));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private long GetForLastDays(int daysCount, SpentStatiscticData data)
        {
            var now = DateTime.Now;
            return data.GetSpentInPeriod(now.AddDays(-daysCount), now);
        }

        public string GetPeriodTitle()
        {
            switch (PeriodFilterType)
            {
                case PeriodFilterTypeEnum.None:
                    return "All time";
                case PeriodFilterTypeEnum.Last30Days:
                    return "Last 30 days";
                case PeriodFilterTypeEnum.Last60Days:
                    return "Last 60 days";
                case PeriodFilterTypeEnum.Last90Days:
                    return "Last 90 days";
                case PeriodFilterTypeEnum.CustomRange:
                    return String.Format("from {0} to {1}", StartDate, EndDate);
                case PeriodFilterTypeEnum.CustomMonthly:
                    return new DateTime(Year.Value, Month.Value, 1).ToString("MMMM");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}