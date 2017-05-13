using System.Collections.Generic;

namespace mPower.Domain.Application.Affiliate.Data
{
    public class CampaignStatisticData
    {
        public long UsersImpressions { get; set; }

        public List<string> AcceptedByUsers { get; set; }

        public long Purchases { get; set; }


        public CampaignStatisticData()
        {
            AcceptedByUsers = new List<string>();
        }
    }
}