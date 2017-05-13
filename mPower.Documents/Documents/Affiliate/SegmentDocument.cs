using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Enums;

namespace mPower.Documents.Documents.Affiliate
{
    public class SegmentDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public List<SegmentOption> Options { get; set; }

        public GenderEnum? Gender { get; set; }

        public string State { get; set; }

        public DateRangeEnum? DateRange { get; set; }

        public DateTime? CustomDateRangeStart { get; set; }

        public DateTime? CustomDateRangeEnd { get; set; }

        public int? AgeRangeFrom {get; set;}

        public int? AgeRangeTo { get; set; }

        public List<string> ZipCodes { get; set; }

        public List<MerchantSelectItem> MerchantSelections { get; set; }

        public List<string> SpendingCategories { get; set; }

        public List<SegmentUserData> MatchedUsers { get; set; }

        public int EstimatedReach
        {
            get { return MatchedUsers.Count; }
        }

        public float Past30DaysGrowthInPct { get; set; }

        public float Past60DaysGrowthInPct { get; set; }

        public float Past90DaysGrowthInPct { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public SegmentDocument()
        {
            MerchantSelections = new List<MerchantSelectItem>();
            SpendingCategories = new List<string>();
            Options = new List<SegmentOption>();
            MatchedUsers = new List<SegmentUserData>();
        }
    }
}