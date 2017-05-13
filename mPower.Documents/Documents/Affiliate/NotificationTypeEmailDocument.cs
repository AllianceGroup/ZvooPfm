using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Application.Enums;

namespace mPower.Documents.Documents.Affiliate
{
    public class NotificationTypeEmailDocument
    {
        [BsonId]
        public EmailTypeEnum EmailType { get; set; }

        public string Name { get; set; }

        public string EmailContentId { get; set; }

        public TriggerStatusEnum Status { get; set; }
    }
}