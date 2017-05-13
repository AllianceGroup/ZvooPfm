using System.Collections.Generic;

namespace Default.Areas.Administration.Models
{
    public class SegmentsListItemModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Reach { get; set; }
        public List<SegmentOptionModel> AllOptions { get; set; }
        public float Past30DaysGrowthInPct { get; set; }
        public float Past60DaysGrowthInPct { get; set; }
        public float Past90DaysGrowthInPct { get; set; }
        public List<string> BasicOptions { get; set; }
    }
}