using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_LogInCommandHandler : IMessageHandler<User_LogInCommand>
    {
        private readonly IRepository _repository;

        public User_LogInCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_LogInCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.LogIn(message.LogInDate, message.AuthToken, message.AffiliateName, message.AffiliateId, message.UserEmail, message.UserName);

            _repository.Save(user);
        }
    }
}
