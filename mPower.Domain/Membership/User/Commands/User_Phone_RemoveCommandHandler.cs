using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Phone_RemoveCommandHandler : IMessageHandler<User_Phone_RemoveCommand>
    {
        private readonly IRepository _repository;

        public User_Phone_RemoveCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_Phone_RemoveCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.RemovePhone(message.Phone);
            _repository.Save(user);
        }
    }
}