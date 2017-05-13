using mPower.Domain.Application.Affiliate.Data;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Campaign_Settings_UpdateCommand: Command
    {
        public string AffiliateId { get; set; }
        public string CampaignId { get; set; }
        public CampaignSettingsData Settings { get; set; }
    }
}