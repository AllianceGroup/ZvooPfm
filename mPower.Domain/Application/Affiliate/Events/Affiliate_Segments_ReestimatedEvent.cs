using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Paralect.Domain;
using mPower.Domain.Application.Affiliate.Data;

namespace mPower.Domain.Application.Affiliate.Events
{
    [BsonIgnoreExtraElements]
    public class Affiliate_Segments_ReestimatedEvent : Event
    {
        public string AffiliateId { get; set; }

        public List<SegmentData> ReestimatedSegments { get; set; }
    }
}