using Paralect.Domain;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_Email_Content_ChangedEvent : Event
    {
        public string Id { get; set; }

        public string TemplateId { get; set; }

        public string AffiliateId { get; set; }

        public string Name { get; set; }

        public string Subject { get; set; }

        public string Html { get; set; }

        public TemplateStatusEnum Status { get; set; }
    }
}