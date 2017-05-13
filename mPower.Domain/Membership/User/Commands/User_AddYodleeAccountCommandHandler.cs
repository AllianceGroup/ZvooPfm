using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_AddYodleeAccountCommandHandler: IMessageHandler<User_AddYodleeAccountCommand>
    {
        private readonly IRepository _repository;

        public User_AddYodleeAccountCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_AddYodleeAccountCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            
            user.AddYodleeAccount(new YodleeSignUpData()
            {
               EmailAddress = message.EmailAddress,
               LastLoginTime = message.LastLoginTime,
               LoginCount = message.LoginCount,
               LoginName = message.LoginName,
               Password = message.Password,
               PasswordChangedOn = message.PasswordChangedOn,
               PasswordExpiryDays = message.PasswordExpiryDays,
               PasswordExpiryNotificationDays = message.PasswordExpiryNotificationDays,
               PasswordRecovered = message.PasswordRecovered,
               UserType = message.UserType
            });

            _repository.Save(user);
        }
    }
}
