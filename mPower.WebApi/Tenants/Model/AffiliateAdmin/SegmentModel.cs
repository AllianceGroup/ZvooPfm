using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Default.Areas.Administration.Models;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Enums;

namespace mPower.WebApi.Tenants.Model.AffiliateAdmin
{
    public class SegmentModel
    {
        public const int DynamicItemsLimit = 5;

        public string Id { get; set; }
        public string AffiliateId { get; set; }

        [Required]
        public string Name { get; set; }
        public int? Reach { get; set; }
        public string ReachFormatted { get; set; }
        public bool IsNew { get; set; }

        public List<SegmentOptionModel> ApplicationOptions { get; set; }

        public GenderEnum? Gender { get; set; }
        public int? AgeRangeFrom { get; set; }
        public int? AgeRangeTo { get; set; }
        public string State { get; set; }
        public List<string> ZipCodes { get; set; }

        public DateRangeEnum? DateRange { get; set; }
        public DateTime? CustomDateRangeStart { get; set; }
        public DateTime? CustomDateRangeEnd { get; set; }

        public List<SegmentOptionModel> FinancesOptions { get; set; }

        public List<MerchantSelectItem> MerchantSelections { get; set; }
        public List<SegmentOptionModel> MerchantOptions { get; set; }

        public List<string> SpendingCategories { get; set; }
        public List<SegmentOptionModel> SpendingCategoryOptions { get; set; }

        public List<SegmentOptionModel> AllOptions
        {
            get
            {
                return ApplicationOptions.Union(FinancesOptions).Union(MerchantOptions).Union(SpendingCategoryOptions).ToList();
            }
        }
    }
}
