using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Domain.Application.Affiliate.Data
{
    [BsonIgnoreExtraElements]
    public class CampaignSettingsData
    {
        public int? MaxPurchases { get; set; }
        public int? DailyPurchaseLimit { get; set; } 
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublic { get; set; }
        public long? CrossAffiliateRedeemCostInCents { get; set; }

        public List<string> Merchants { get; set; }
        public string Category { get; set; }

        public string Email { get; set; }
        public bool NotifyPerPurchase { get; set; }
        public int? NotificationPurchasesCountBorderValue { get; set; }
        public PurchaseSummaryReportPeriodEnum? PurchaseSummaryReportPeriod { get; set; }
    }
}