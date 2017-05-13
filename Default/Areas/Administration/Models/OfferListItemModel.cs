using System;
using mPower.Domain.Application.Enums;

namespace Default.Areas.Administration.Models
{
    public class OfferListItemModel
    {
        public string CampaignId { get; set; }
        public string Headline { get; set; }
        public string Body { get; set; }
        public OfferTypeEnum Type { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Merchant { get; set; }
        public string Category { get; set; }
        public float? OfferValueInPerc { get; set; }
        public long? OfferValueInCents { get; set; }
        public long CrossAffiliateRedeemCostInCents { get; set; }
        public string LogoPath { get; set; }
        public int RedeemedCount { get; set; }
        public int? RedeemedMax { get; set; }
    }
}