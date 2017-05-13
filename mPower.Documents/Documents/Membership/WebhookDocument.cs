using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Membership
{
    public class WebhookDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string SubscriptionId { get; set; }

        public string CustomerId { get; set; }

        public string ChargifyWebhookId { get; set; }

        public DateTime Date { get; set; }

        public Dictionary<string, string> WebhookParams { get; set; }

        public string WebhookEvent { get; set; }
    }
}
