using mPower.Domain.Application.Affiliate.Data;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Campaign_CreateCommand: Command
    {
        public string AffiliateId { get; set; }
        public string CampaignId { get; set; }
        public string SegmentId { get; set; }
        public CampaignOfferData Offer { get; set; }
    }
}