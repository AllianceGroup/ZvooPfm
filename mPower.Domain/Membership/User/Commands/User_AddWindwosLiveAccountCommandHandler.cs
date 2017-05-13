using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_AddWindwosLiveAccountCommandHandler : IMessageHandler<User_AddWindwosLiveAccountCommand>
    {
        private readonly IRepository _repository;

        public User_AddWindwosLiveAccountCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_AddWindwosLiveAccountCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.AddWindowsLiveAccount(new WindowsLiveSignupData()
            {
                DisplayName = message.DisplayName,
                FormattedName = message.FormattedName,
                Identifier = message.Identifier,
                PreferredUsername = message.PreferredUsername,
                ProfileUrl = message.ProfileUrl,
                Email = message.Email,
                FamilyName = message.FamilyName,
                GivenName = message.GivenName
            });

            _repository.Save(user);
        }
    }
}
