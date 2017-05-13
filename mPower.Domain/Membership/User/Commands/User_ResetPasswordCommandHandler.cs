using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_ResetPasswordCommandHandler : IMessageHandler<User_ResetPasswordCommand>
    {
        private readonly IRepository _repository;

        public User_ResetPasswordCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_ResetPasswordCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.ResetPassword(message.PasswordHash, message.ChangeDate);

            _repository.Save(user);
        }
    }
}
