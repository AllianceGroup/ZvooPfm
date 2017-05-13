using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_AddTwitterAccountCommandHandler : IMessageHandler<User_AddTwitterAccountCommand>
    {
        private readonly IRepository _repository;

        public User_AddTwitterAccountCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_AddTwitterAccountCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.AddTwitterAccount(new TwitterSignupData()
            {
                DisplayName = message.DisplayName,
                FormattedName = message.FormattedName,
                Identifier = message.Identifier,
                PreferredUsername = message.PreferredUsername,
                ProfileUrl = message.ProfileUrl,
                PhotoUrl = message.PhotoUrl
            });

            _repository.Save(user);
        }
    }
}
