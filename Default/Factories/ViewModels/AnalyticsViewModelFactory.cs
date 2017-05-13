using Default.Areas.Administration.Models;
using mPower.Documents.Documents.Membership;
using mPower.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Default.Factories.ViewModels
{
    public enum StatisticTypeEnum
    {
        Total = 0,
        Average = 1,
        AverageAnnual = 2,
    }

    public class AnalyticsViewModelDto
    {
        public Dictionary<string, SpentStatiscticData> Items { get; set; }
        public StatisticTypeEnum StatisticType { get; set; }

        public AnalyticsViewModelDto(Dictionary<string, SpentStatiscticData> items, StatisticTypeEnum statisticType = StatisticTypeEnum.Total)
        {
            Items = items;
            StatisticType = statisticType;
        }
    }

    public class AnalyticsViewModelFactory : 
        IObjectFactory<SpentStatiscticData, AnalyticsModel>,
        IObjectFactory<AnalyticsViewModelDto, AnalyticsModel>
    {
        private readonly DateTime _now = DateTime.Now;

        public AnalyticsModel Load(SpentStatiscticData items)
        {
            return BuildAnalyticModel(items.Total, daysAgo => CalculateTotalForLastDays(items, daysAgo));
        }

        public AnalyticsModel Load(AnalyticsViewModelDto dto)
        {
            switch (dto.StatisticType)
            {
                case StatisticTypeEnum.Total:
                    return BuildAnalyticModel(dto.Items.Values.Sum(x => x.Total), daysAgo => CalculateTotalForLastDays(dto.Items, daysAgo));

                case StatisticTypeEnum.Average:
                    var avg = dto.Items.Any()
                        ? dto.Items.Values.Sum(x => x.Total)/dto.Items.Count
                        : 0;
                    return BuildAnalyticModel(avg, daysAgo => CalculateAvgForLastDays(dto.Items, daysAgo));

                case StatisticTypeEnum.AverageAnnual:
                    var total = CalculateAvgAnnualForDate(dto.Items, 0);
                    return BuildAnalyticModel(total, daysAgo => total - CalculateAvgAnnualForDate(dto.Items, daysAgo));

                default:
                    throw new NotSupportedException();
            }
        }

        private static AnalyticsModel BuildAnalyticModel(long total, Func<int, long> getLastDaysStatistic)
        {
            var last30Days = getLastDaysStatistic(30);
            var last60Days = getLastDaysStatistic(60);
            var last90Days = getLastDaysStatistic(90);

            if (total > 0)
            {
                var result = new AnalyticsModel
                {
                    Amount = total / 100,
                    Past30DaysGrossing = GetGrossingPerc(total - last30Days, last30Days),
                    Past60DaysGrossing = GetGrossingPerc(total - last60Days, last60Days),
                    Past90DaysGrossing = GetGrossingPerc(total - last90Days, last90Days),
                };

                return result;
            }

            return new AnalyticsModel();
        }

        private static int GetGrossingPerc(long value, long inc)
        {
            return Math.Abs(value) > 0 ? (int) (inc*100/(decimal) (value)) : 0;
        }

        private long CalculateAvgAnnualForDate(Dictionary<string, SpentStatiscticData> docs, int daysAgo)
        {
            var endDate = _now.AddDays(-daysAgo);
            var startDate = endDate.AddYears(-1);

            var perUser = new List<double>();
            foreach (var stat in docs.Values)
            {
                var minDate = stat.GetMinDate(startDate, endDate);
                if (minDate.HasValue)
                {
                    var amount = stat.GetSpentInPeriod(startDate, endDate) * 365f / ((endDate - minDate.Value).TotalDays + 1);
                    perUser.Add(amount);
                }
            }
            var count = perUser.Count;
            return (count != 0) ? (long)perUser.Sum(x => x) / count : 0;
        }

        private long CalculateAvgForLastDays(Dictionary<string, SpentStatiscticData> docs, int daysAgo)
        {
            var startDate = _now.AddDays(-daysAgo);

            var perUser = new List<double>();
            foreach (var stat in docs.Values)
            {
                var minDate = stat.GetMinDate(startDate, _now);
                if (minDate.HasValue)
                {
                    var amount = stat.GetSpentInPeriod(startDate, _now);
                    perUser.Add(amount);
                }
            }
            var count = perUser.Count;
            return (count != 0) ? (long)perUser.Sum(x => x) / count : 0;
        }

        private long CalculateTotalForLastDays(Dictionary<string, SpentStatiscticData> docs, int daysAgo)
        {
            var startDate = _now.AddDays(-daysAgo);

            return docs.Values.Sum(x => x.GetSpentInPeriod(startDate, _now));
        }

        private long CalculateTotalForLastDays(SpentStatiscticData docs, int daysAgo)
        {
            var startDate = _now.AddDays(-daysAgo);

            return docs.GetSpentInPeriod(startDate, _now);
        }
    }
}