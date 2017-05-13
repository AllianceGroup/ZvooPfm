using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Subscription_CreateCommandHandler : IMessageHandler<User_Subscription_CreateCommand>
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public User_Subscription_CreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_Subscription_CreateCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.AddSubscription(message.SubscriptionId, message.CreditIdentityId);

            _repository.Save(user);
        }
    }
}
