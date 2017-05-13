using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Phone_AddCommandHandler : IMessageHandler<User_Phone_AddCommand>
    {
        private readonly IRepository _repository;

        public User_Phone_AddCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_Phone_AddCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.AddPhone(message.Phone);
            _repository.Save(user);
        }
    }
}