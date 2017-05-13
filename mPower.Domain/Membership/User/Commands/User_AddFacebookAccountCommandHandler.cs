using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_AddFacebookAccountCommandHandler : IMessageHandler<User_AddFacebookAccountCommand>
    {
        private readonly IRepository _repository;

        public User_AddFacebookAccountCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_AddFacebookAccountCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.AddFacebookAccount(new FacebookSignupData()
            {
                DisplayName = message.DisplayName,
                Email = message.Email,
                FamilyName = message.FamilyName,
                FormattedName = message.FormattedName,
                Gender = message.Gender,
                GivenName = message.GivenName,
                Identifier = message.Identifier,
                PhotoUrl = message.PhotoUrl,
                PreferredUsername = message.PreferredUsername,
                ProfileUrl = message.ProfileUrl,
                VerifiedEmail = message.VerifiedEmail
            });

            _repository.Save(user);
        }
    }
}
