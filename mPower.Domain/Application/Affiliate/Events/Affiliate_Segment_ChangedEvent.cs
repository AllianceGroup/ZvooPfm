using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Enums;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Events
{
    [BsonIgnoreExtraElements]
    public class Affiliate_Segment_ChangedEvent : Event
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

        public DateTime UpdatedDate { get; set; }

        public float Past30DaysGrowthInPct { get; set; }

        public float Past60DaysGrowthInPct { get; set; }

        public float Past90DaysGrowthInPct { get; set; }


        public Affiliate_Segment_ChangedEvent()
        {
            ZipCodes = new List<string>();
            MerchantSelections = new List<MerchantSelectItem>();
        }
    }
}