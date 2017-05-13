using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_UpdateResetPasswordTokenCommandHandler : IMessageHandler<User_UpdateResetPasswordTokenCommand>
    {
        private readonly IRepository _repository;

        public User_UpdateResetPasswordTokenCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_UpdateResetPasswordTokenCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.UpdateResetPasswordToken(message.Token);
            _repository.Save(user);
        }
    }
}
