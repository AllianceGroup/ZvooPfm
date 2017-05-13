using System;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Application.Enums;

namespace mPower.Documents.Documents.Affiliate
{
    public class EmailContentDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public string TemplateId { get; set; }

        public string Subject { get; set; }

        public string Html { get; set; }

        public EmailTypeEnum? IsDefaultForEmailType { get; set; }

        public DateTime CreationDate { get; set; }

        public TemplateStatusEnum Status { get; set; }
    }
}