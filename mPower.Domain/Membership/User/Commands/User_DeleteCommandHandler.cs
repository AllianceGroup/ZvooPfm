using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_DeleteCommandHandler : IMessageHandler<User_DeleteCommand>
    {
        private readonly IRepository _repository = null;

        public User_DeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_DeleteCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.Delete();

            _repository.Save(user);
        }
    }
}
