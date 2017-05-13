using System;
using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_TwitterSignupCommandHandler : IMessageHandler<User_TwitterSignupCommand>
    {
        private readonly IRepository _repository;

        public User_TwitterSignupCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_TwitterSignupCommand message)
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

            user.TwitterSignup(new TwitterSignupData
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
