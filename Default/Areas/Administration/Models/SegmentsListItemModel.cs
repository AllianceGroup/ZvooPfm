using System.Collections.Generic;

namespace Default.Areas.Administration.Models
{
    public class SegmentsListItemModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Reach { get; set; }
        public List<SegmentOptionModel> AllOptions { get; set; }
        public double Past30DaysGrowthInPct { get; set; }
        public double Past60DaysGrowthInPct { get; set; }
        public double Past90DaysGrowthInPct { get; set; }
        public List<string> BasicOptions { get; set; }
    }
}