using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_Faq_DeletedEvent : Event
    {
        public string Id { get; set; }

        public string AffiliateId { get; set; }
    }
}
