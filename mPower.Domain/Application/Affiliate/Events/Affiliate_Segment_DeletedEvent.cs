using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_Segment_DeletedEvent: Event
    {
        public string AffiliateId { get; set; }

        public string Id { get; set; }
    }
}