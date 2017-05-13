using mPower.Domain.Application.Affiliate.Data;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Campaign_Offer_UpdateCommand: Command
    {
        public string AffiliateId { get; set; }
        public string CampaignId { get; set; }
        public CampaignOfferData Offer { get; set; }
    }
}