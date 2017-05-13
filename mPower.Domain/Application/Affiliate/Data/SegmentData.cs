using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Application.Affiliate.Data
{
    public class SegmentUserData
    {
        public string Id { get; set; }
        public string AffiliateId { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class SegmentData
    {
        public string Id { get; set; }

        public string AffiliateId { get; set; }

        public string Name { get; set; }

        public List<SegmentOption> Options { get; set; }

        public GenderEnum? Gender { get; set; }

        public string State { get; set; }

        public DateRangeEnum? DateRange { get; set; }

        public DateTime? CustomDateRangeStart { get; set; }

        public DateTime? CustomDateRangeEnd { get; set; }

        public int? AgeRangeFrom { get; set; }

        public int? AgeRangeTo { get; set; }

        public List<string> ZipCodes { get; set; }

        public List<MerchantSelectItem> MerchantSelections { get; set; }

        public List<string> SpendingCategories { get; set; }

        public List<SegmentUserData> MatchedUsers { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public float Past30DaysGrowthInPct { get; set; }

        public float Past60DaysGrowthInPct { get; set; }

        public float Past90DaysGrowthInPct { get; set; }
    }
}