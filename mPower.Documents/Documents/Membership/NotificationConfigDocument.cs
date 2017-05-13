using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Application.Enums;

namespace mPower.Documents.Documents.Membership
{
    public class NotificationConfigDocument
    {
        [BsonId]
        public EmailTypeEnum Type { get; set; }

        public bool SendEmail { get; set; }

        public bool SendText { get; set; }

        public int BorderValue { get; set; }
    }
}
