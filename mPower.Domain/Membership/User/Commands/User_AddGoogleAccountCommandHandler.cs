using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_AddGoogleAccountCommandHandler : IMessageHandler<User_AddGoogleAccountCommand>
    {
        private readonly IRepository _repository;

        public User_AddGoogleAccountCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_AddGoogleAccountCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.AddGoogleAccount(new GoogleSignupData()
            {
                DisplayName = message.DisplayName,
                Email = message.Email,
                FamilyName = message.FamilyName,
                FormattedName = message.FormattedName,
                GivenName = message.GivenName,
                Identifier = message.Identifier,
                PreferredUsername = message.PreferredUsername,
                ProfileUrl = message.ProfileUrl,
                VerifiedEmail = message.VerifiedEmail,
                GoogleUserId = message.GoogleUserId
            });

            _repository.Save(user);
        }
    }
}
