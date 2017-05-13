using System;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Application.Affiliate.Data
{
    public class EmailTemplateData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Html { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreationDate { get; set; }

        public TemplateStatusEnum Status { get; set; }
    }
}