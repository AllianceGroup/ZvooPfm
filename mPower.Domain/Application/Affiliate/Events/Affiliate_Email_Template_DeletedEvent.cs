using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_Email_Template_DeletedEvent : Event
    {
        public string Id { get; set; }

        public string AffiliateId { get; set; }
    }
}