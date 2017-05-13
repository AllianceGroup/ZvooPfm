using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_MobileLogInCommandHandler : IMessageHandler<User_MobileLogInCommand>
    {
        private readonly IRepository _repository;

        public User_MobileLogInCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_MobileLogInCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.LogInMobile(message.LogInDate, message.AccessToken, message.AffiliateName, message.AffiliateId, message.UserEmail, message.UserName);

            _repository.Save(user);
        }
    }
}
