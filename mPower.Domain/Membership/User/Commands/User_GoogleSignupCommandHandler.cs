using System;
using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_GoogleSignupCommandHandler : IMessageHandler<User_GoogleSignupCommand>
    {
        private readonly IRepository _repository;

        public User_GoogleSignupCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_GoogleSignupCommand message)
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
            user.GoogleSignup(new GoogleSignupData
            {
                DisplayName = message.DisplayName,
                Email = message.Email,
                FamilyName = message.FamilyName,
                FormattedName = message.FormattedName,
                GivenName = message.GivenName,
                GoogleUserId = message.GoogleUserId,
                Identifier = message.Identifier,
                PreferredUsername = message.PreferredUsername,
                ProfileUrl = message.ProfileUrl,
                VerifiedEmail = message.VerifiedEmail
            });

            _repository.Save(user);
        }
    }
}
