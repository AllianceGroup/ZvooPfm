using System;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Application.Enums;

namespace mPower.TempDocuments.Server.Notifications.Documents
{
    [BsonIgnoreExtraElements]
    public class BaseNotification
    {
        [BsonId]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string AffiliateId { get; set; }

        public EmailTypeEnum Type { get; set; }
        public string PublicKey { get; set; }

        public DateTime SendDate { get; set; }

        public bool SendEmail { get; set; }
        public bool SendText { get; set; }

        public bool InProgress { get; set; }
    }
}