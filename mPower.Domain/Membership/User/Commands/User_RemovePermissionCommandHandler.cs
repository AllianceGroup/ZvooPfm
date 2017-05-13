using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_RemovePermissionCommandHandler : IMessageHandler<User_RemovePermissionCommand>
    {
        private readonly IRepository _repository;

        public User_RemovePermissionCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_RemovePermissionCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.RemovePermission(message.Permission);

            _repository.Save(user);
        }
    }
}
