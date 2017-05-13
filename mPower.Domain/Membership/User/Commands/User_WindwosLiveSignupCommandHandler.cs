using System;
using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_WindwosLiveSignupCommandHandler : IMessageHandler<User_WindwosLiveSignupCommand>
    {
        private readonly IRepository _repository;

        public User_WindwosLiveSignupCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_WindwosLiveSignupCommand message)
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

            user.WindowsLiveSignup(new WindowsLiveSignupData
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
