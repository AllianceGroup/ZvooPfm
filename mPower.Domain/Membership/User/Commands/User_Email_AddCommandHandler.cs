using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Email_AddCommandHandler : IMessageHandler<User_Email_AddCommand>
    {
        private readonly IRepository _repository;

        public User_Email_AddCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_Email_AddCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.AddEmail(message.Email);
            _repository.Save(user);
        }
    }
}