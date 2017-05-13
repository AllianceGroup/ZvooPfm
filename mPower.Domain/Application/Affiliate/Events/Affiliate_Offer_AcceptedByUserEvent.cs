using System;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_Offer_AcceptedByUserEvent : Event
    {
        public string UserAffiliateId { get; set; }

        public string UserId { get; set; }

        public string OfferAffiliateId { get; set; }

        public string OfferId { get; set; }

        public DateTime Date { get; set; }
    }
}