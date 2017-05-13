using System;
using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_FacebookSignupCommandHandler : IMessageHandler<User_FacebookSignupCommand>
    {
        private readonly IRepository _repository;

        public User_FacebookSignupCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_FacebookSignupCommand message)
        {
            var data = new UserData
            {
                UserId = message.UserId,
                FirstName = message.SignupFirstName,
                LastName = message.SignupLastName,
                Email = message.SignupEmail,
                UserName = message.SignupUserName,
                PasswordHash = string.Empty,
                CreateDate = DateTime.Now,
                IsActive = true,
                ApplicationId = message.ApplicationId,
                Metadata = message.Metadata,
                ReferralCode = message.ReferralCode,
            };

            var user = new UserAR(data);

            user.FacebookSignup(new FacebookSignupData
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
                Gender = message.Gender,
                PhotoUrl = message.PhotoUrl
            });

            _repository.Save(user);
        }
    }
}
