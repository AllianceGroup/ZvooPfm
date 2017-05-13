using Paralect.ServiceBus;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Membership.User.Events;
using mPower.Framework;

namespace mPower.EventHandlers.Eventual
{
    public class WebhookDocumentEventHandler : IMessageHandler<User_Subscription_ReceivedWebhookEvent>
    {
        private readonly MongoRead _mongoRead;

        public WebhookDocumentEventHandler(MongoRead mongoRead)
        {
            _mongoRead = mongoRead;
        }

        public void Handle(User_Subscription_ReceivedWebhookEvent message)
        {
            _mongoRead.WebHooks.Insert(new WebhookDocument()
            {
                ChargifyWebhookId = message.ChargifyWebhookId,
                CustomerId = message.CustomerId,
                Date = message.Date,
                Id = message.Id,
                SubscriptionId = message.SubscriptionId,
                WebhookEvent = message.WebhookEvent,
                WebhookParams = message.WebhookParams,
            });
        }
    }
}
