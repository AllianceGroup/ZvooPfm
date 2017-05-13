using System;
using System.Collections.Generic;
using mPower.Domain.Application.Affiliate.Data;

namespace mPower.Documents.Segments
{
    public class SegmentEstimationHelper
    {
        private readonly SegmentAggregationService _segmentAggregationService;

        public SegmentEstimationHelper(SegmentAggregationService segmentAggregationService)
        {
            _segmentAggregationService = segmentAggregationService;
        }

        public List<SegmentUserData> GetMatchingUsers(SegmentData data, DateTime? onDate = null)
        {
            return _segmentAggregationService.GetSegmentUser(data, onDate);
        }

        public List<SegmentUserData> CalculatedSegmentGrowth(SegmentData data, out float past30Prc, out float past60Prc, out float past90Prc)
        {
            var today = DateTime.Today;
            var currentReach = GetMatchingUsers(data);
            var reachWithoutPast30Days = GetMatchingUsers(data, today.AddDays(-30));
            var reachWithoutPast60Days = GetMatchingUsers(data, today.AddDays(-60));
            var reachWithoutPast90Days = GetMatchingUsers(data, today.AddDays(-90));

            past30Prc = (currentReach.Count - reachWithoutPast30Days.Count) * 100f / Math.Max(reachWithoutPast30Days.Count, 1);
            past60Prc = (currentReach.Count - reachWithoutPast60Days.Count) * 100f / Math.Max(reachWithoutPast60Days.Count, 1);
            past90Prc = (currentReach.Count - reachWithoutPast90Days.Count) * 100f / Math.Max(reachWithoutPast90Days.Count, 1);

            return currentReach;
        }
    }
}