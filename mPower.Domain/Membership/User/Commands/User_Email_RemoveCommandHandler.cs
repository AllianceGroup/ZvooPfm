using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Email_RemoveCommandHandler : IMessageHandler<User_Email_RemoveCommand>
    {
        private readonly IRepository _repository;

        public User_Email_RemoveCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_Email_RemoveCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.RemoveEmail(message.Email);
            _repository.Save(user);
        }
    }
}