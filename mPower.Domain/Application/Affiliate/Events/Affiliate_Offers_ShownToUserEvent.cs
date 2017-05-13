using System.Collections.Generic;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_Offers_ShownToUserEvent : Event
    {
        public string UserAffiliateId { get; set; }

        public string UserId { get; set; }

        public Dictionary<string, List<string>> ShownAffiliateOffers { get; set; } 
    }
}