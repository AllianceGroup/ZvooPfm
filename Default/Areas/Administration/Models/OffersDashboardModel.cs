using System.Collections.Generic;

namespace Default.Areas.Administration.Models
{
    public class OffersDashboardModel
    {
        public long Impressions { get; set; }

        public long Clicks { get; set; }

        public long Purchases { get; set; }

        public float PurchaseRate
        {
            get { return Clicks == 0 ? 0 : Purchases/(float) Clicks; }
        }

        public float EngagementRate
        {
            get { return Impressions == 0 ? 0 : Clicks/(float) Impressions; }
        }

        public List<OfferListItemShortModel> TopActiveOffers { get; set; }


        public OffersDashboardModel()
        {
            TopActiveOffers = new List<OfferListItemShortModel>();
        }
    }
}