using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_DeactivateCommandHandler : IMessageHandler<User_DeactivateCommand>
    {
        private readonly IRepository _repository = null;

        public User_DeactivateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_DeactivateCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.Deacivate(message.IsAdmin);

            _repository.Save(user);
        }
    }
}
