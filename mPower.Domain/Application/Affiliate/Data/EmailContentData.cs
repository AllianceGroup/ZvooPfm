using System;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Application.Affiliate.Data
{
    public class EmailContentData
    {
        public string Id { get; set; }

        public string TemplateId { get; set; }

        public string Name { get; set; }

        public string Subject { get; set; }

        public string Html { get; set; }

        public EmailTypeEnum? IsDefaultForEmailType { get; set; }

        public DateTime CreationDate { get; set; }

        public TemplateStatusEnum Status { get; set; }
    }
}