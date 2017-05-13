using System;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Enums;

namespace mPower.Documents.Documents.Affiliate
{
    public class CampaignDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string SegmentId { get; set; }

        public OfferStatusEnum Status { get; set; }

        public CampaignOfferData Offer { get; set; }

        public CampaignSettingsData Settings { get; set; }

        public CampaignStatisticData Statistic { get; set; }

        public bool IsPublic
        {
            get { return Settings != null && Settings.IsPublic; }
        }

        private bool IsActive
        {
            get
            {
                return Settings != null 
                    && (!Settings.StartDate.HasValue || Settings.StartDate.Value <= DateTime.Today)
                    && (!Settings.EndDate.HasValue || Settings.EndDate.Value >= DateTime.Today);
            }
        }

        public string Merchant
        {
            get
            {
                var anyMerchantSpecified = Settings != null && Settings.Merchants != null && Settings.Merchants.Any();
                return anyMerchantSpecified ? string.Join(", ", Settings.Merchants) : "";
            }
        }

        public string Category
        {
            get
            {
                return Settings != null && !string.IsNullOrEmpty(Settings.Category) ? Settings.Category : "";
            }
        }


        public CampaignDocument()
        {
            // TODO: status should be Pending by default. Apply it when there will be an ability to change status from Pending to Active from UI
            Status = OfferStatusEnum.Active;
            Statistic = new CampaignStatisticData();
        }

        #region Methods

        public bool MatchesMerchantName(string merchant)
        {
            return !string.IsNullOrEmpty(merchant) && Settings != null && Settings.Merchants != null
                && Settings.Merchants.Any(x =>  merchant.ToLowerInvariant().Contains(x.ToLowerInvariant()));
        }

        public bool MatchesCategory(params string [] categories)
        {
            return categories != null && Settings != null && !string.IsNullOrEmpty(Settings.Category) && categories.Any(ctg => Settings.Category.Equals(ctg));
        }

        public OfferStatusEnum GetCurrentStatus()
        {
            return Status == OfferStatusEnum.Pending || IsActive
                ? Status
                : OfferStatusEnum.Expired;
        }

        #endregion
    }
}