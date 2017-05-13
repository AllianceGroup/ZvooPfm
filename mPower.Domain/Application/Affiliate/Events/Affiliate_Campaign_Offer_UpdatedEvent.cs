using mPower.Domain.Application.Affiliate.Data;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_Campaign_Offer_UpdatedEvent : Event
    {
        public string AffiliateId { get; set; }
        public string CampaignId { get; set; }
        public CampaignOfferData Offer { get; set; }
    }
}