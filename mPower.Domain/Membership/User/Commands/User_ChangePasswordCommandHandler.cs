using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_ChangePasswordCommandHandler : IMessageHandler<User_ChangePasswordCommand>
    {
        private readonly IRepository _repository;

        public User_ChangePasswordCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_ChangePasswordCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.ChangePassword(message.PasswordHash, message.ChangeDate);

            _repository.Save(user);
        }
    }
}
