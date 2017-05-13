using System;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Offer_AcceptByUserCommand : Command
    {
        public string UserAffiliateId { get; set; }

        public string UserId { get; set; }

        public string OfferAffiliateId { get; set; }

        public string OfferId { get; set; }

        public DateTime Date { get; set; } 
    }
}