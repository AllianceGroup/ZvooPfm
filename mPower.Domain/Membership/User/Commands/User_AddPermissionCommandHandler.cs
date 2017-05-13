using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_AddPermissionCommandHandler : IMessageHandler<User_AddPermissionCommand>
    {
        private readonly IRepository _repository;

        public User_AddPermissionCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_AddPermissionCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.AddPermission(message.Permission);

            _repository.Save(user);
        }
    }
}
