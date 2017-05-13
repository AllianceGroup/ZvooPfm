using System;
using mPower.Domain.Application.Enums;

namespace mPower.Documents.Documents.Membership
{
    public class RedeemedOfferDocument
    {
       
        public string Id { get; set; }
        public DateTime RedeemedDate { get; set; }
        public OfferTypeEnum? OfferType { get; set; }
        public bool IsCampaign { get; set; }
        public string OfferAffiliateId { get; set; }
        public float? ValueInPerc { get; set; }
        public long? ValueInCents { get; set; }

        public RedeemedOfferDocument()
        {
            
        }

        public RedeemedOfferDocument(string offerId, DateTime date, string offerAffiliateId, OfferTypeEnum offerType, long? offerValueInCents, float? offerValueInPerc)
        {
            Id = offerId;
            OfferAffiliateId = offerAffiliateId;
            RedeemedDate = date;
            OfferType = offerType;
            ValueInCents = offerValueInCents;
            ValueInPerc = offerValueInPerc;
            IsCampaign = true;
        }

    }
}