using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Subscription_DeleteCommandHandler : IMessageHandler<User_Subscription_DeleteCommand>
    {
        private readonly Repository _repository;

        public User_Subscription_DeleteCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(User_Subscription_DeleteCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.DeleteSubscription(message.CancelMessage, message.SubscriptionId, message.CreditIdentityId);

            _repository.Save(user);
        }
    }
}
