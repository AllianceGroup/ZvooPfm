using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_Faq_UpdatedEvent : Event
    {
        public string Id { get; set; }

        public string AffiliateId { get; set; }

        public string Name { get; set; }

        public string Html { get; set; }

        public bool IsActive { get; set; }
    }
}
