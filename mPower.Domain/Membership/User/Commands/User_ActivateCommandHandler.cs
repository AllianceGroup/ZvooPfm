using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_ActivateCommandHandler : IMessageHandler<User_ActivateCommand>
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public User_ActivateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_ActivateCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.Activate(message.IsAdmin);
            _repository.Save(user);
        }
    }
}
