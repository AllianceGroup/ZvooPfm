using System;
using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Segment_AddCommand: Command
    {
        public string Id { get; set; }

        public string AffiliateId { get; set; }

        public string Name { get; set; }

        public IEnumerable<SegmentOption> Options { get; set; }

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

        public DateTime CreatedDate { get; set; }

        public float Past30DaysGrowthInPct { get; set; }

        public float Past60DaysGrowthInPct { get; set; }

        public float Past90DaysGrowthInPct { get; set; }

    }
}