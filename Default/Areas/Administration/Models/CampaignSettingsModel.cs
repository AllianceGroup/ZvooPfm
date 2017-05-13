using mPower.Domain.Accounting;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Default.Areas.Administration.Models
{
    public class CampaignSettingsModel
    {
        public string CampaignId { get; set; }
        public OfferTypeEnum OfferType { get; set; }

        [Display(Name = "Max Purchases")]
        [Range(1, int.MaxValue)]
        public int? MaxPurchases { get; set; }

        [Display(Name = "Daily Purchase Limit")]
        [Range(1, int.MaxValue)]
        public int? DailyPurchaseLimit { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "make this offer public")]
        public bool IsPublic { get; set; }

        public decimal? CrossAffiliateRedeemCostInDollars { get; set; }

        #region Delivery Options

        public List<string> Merchants { get; set; }

        public string Category { get; set; }

        public List<RootExpenseAccount> SpendingCategories { get; set; }

        #endregion

        #region Notifications

        [Display(Name = "Email Address")]
        [RegularExpression(Constants.Validation.Regexps.Email, ErrorMessage = Constants.Validation.Messages.InvalidEmail)]
        public string Email { get; set; }

        [Display(Name = "notify me every time a purchase is made (note: this can produce a LOT of emails)")]
        public bool NotifyPerPurchase { get; set; }

        [Display(Name = "notify me when purchases exceed")]
        public bool NotifyWhenPurchasesNumberExceeded { get; set; }

        public int? NotificationPurchasesCountBorderValue { get; set; }

        [Display(Name = "send me purchase summary reports each")]
        public bool SendPurchaseSummaryReport { get; set; }

        public PurchaseSummaryReportPeriodEnum PurchaseSummaryReportPeriod { get; set; }

        #endregion
    }
}