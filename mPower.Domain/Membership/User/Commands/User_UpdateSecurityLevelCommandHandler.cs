using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_UpdateSecurityLevelCommandHandler : IMessageHandler<User_UpdateSecurityLevelCommand>
    {
        private readonly IRepository _repository;

        public User_UpdateSecurityLevelCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_UpdateSecurityLevelCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.UpdateSecurityLevel(message.SecurityLevel);

            _repository.Save(user);
        }
    }
}