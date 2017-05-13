using System;
using System.Collections.Generic;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_Subscription_ReceivedWebhookEvent : Event
    {
        public string Id { get; set; }

        public string CustomerId { get; set; }

        public string ChargifyWebhookId { get; set; }

        public DateTime Date { get; set; }

        public Dictionary<string, string> WebhookParams { get; set; }

        public string WebhookEvent { get; set; }

        public string SubscriptionId { get; set; }

        public string UserId { get; set; }
    }
}
