using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_GoalsLinkedAccount_SetCommandHandler : IMessageHandler<User_GoalsLinkedAccount_SetCommand>
    {
        private readonly IRepository _repository;

        public User_GoalsLinkedAccount_SetCommandHandler(IRepository repository)
         {
             _repository = repository;
         }

        public void Handle(User_GoalsLinkedAccount_SetCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.SetGoalsLinkedAccount(message.LedgerId, message.AccountId);

            _repository.Save(user);
        }
    }
};