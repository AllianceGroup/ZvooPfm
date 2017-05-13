using System;
using Paralect.Domain;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_Email_Template_AddedEvent : Event
    {
        public string Id { get; set; }

        public string AffiliateId { get; set; }

        public string Name { get; set; }

        public string Html { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreationDate { get; set; }

        public TemplateStatusEnum Status { get; set; }

        public Affiliate_Email_Template_AddedEvent()
        {
            CreationDate = DateTime.Now;
            Status = TemplateStatusEnum.Active; 
        }
    }
}