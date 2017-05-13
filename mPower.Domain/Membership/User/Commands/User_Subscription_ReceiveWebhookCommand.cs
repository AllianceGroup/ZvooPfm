using System;
using System.Collections.Generic;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Subscription_ReceiveWebhookCommand : Command
    {
        public string UserId { get; set; }

        public string CustomerId { get; set; }

        public string SubscriptionId { get; set; }

        public string ChargifyWebhookId { get; set; }

        public DateTime Date { get; set; }

        public Dictionary<string, string> WebhookParams { get; set; }

        public string WebhookEvent { get; set; }
    }
}
