using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_UpdateCommandHandler : IMessageHandler<User_UpdateCommand>
    {
        private readonly IRepository _repository;

        public User_UpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_UpdateCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.Update(
                message.FirstName, 
                message.LastName, 
                message.Email,
                message.ZipCode,
                message.BirthDate,
                message.Gender,
                message.IsAgent);

            _repository.Save(user);
        }
    }
}
