using System;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Application.Affiliate.Data
{
    [BsonIgnoreExtraElements]
    public class CampaignOfferData
    {
        public OfferTypeEnum OfferType { get; set; }
        public string Name { get; set; }
        public string Headline { get; set; }
        public string Body { get; set; }
        public string Logo { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Terms { get; set; }
        public float? OfferValueInPerc { get; set; }
        public long? OfferValueInCents { get; set; }
    }
}