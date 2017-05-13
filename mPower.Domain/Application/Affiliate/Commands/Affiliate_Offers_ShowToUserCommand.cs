using System.Collections.Generic;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Offers_ShowToUserCommand : Command
    {
        public string UserAffiliateId { get; set; }

        public string UserId { get; set; }

        public Dictionary<string, List<string>> ShownAffiliateOffers { get; set; } 
    }
}