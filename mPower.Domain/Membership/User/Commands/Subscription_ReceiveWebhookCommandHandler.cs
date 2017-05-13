using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class Subscription_ReceiveWebhookCommandHandler : IMessageHandler<User_Subscription_ReceiveWebhookCommand>
    {
        private readonly Repository _repository;

        public Subscription_ReceiveWebhookCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(User_Subscription_ReceiveWebhookCommand message)
        {
            //TODO: Brett Allred Get User by SubscriptionId
            var userAr = _repository.GetById<UserAR>(message.UserId);
            userAr.SetCommandMetadata(message.Metadata);

            userAr.ReceiveWebHook(message.SubscriptionId, message.CustomerId, message.ChargifyWebhookId, message.Date, message.WebhookParams, message.WebhookEvent);

            _repository.Save(userAr);
        }
    }
}
