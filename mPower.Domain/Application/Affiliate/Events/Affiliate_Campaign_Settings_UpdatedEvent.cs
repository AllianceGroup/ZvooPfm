using mPower.Domain.Application.Affiliate.Data;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_Campaign_Settings_UpdatedEvent: Event
    {
        public string AffiliateId { get; set; }
        public string CampaignId { get; set; }
        public CampaignSettingsData Settings { get; set; }
    }
}