using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Realestate_DeleteCommandHandler : IMessageHandler<User_Realestate_DeleteCommand>
    {
         private readonly IRepository _repository;

         public User_Realestate_DeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

         public void Handle(User_Realestate_DeleteCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.DeleteRealestate(message.Id);

            _repository.Save(user);
        }
    }
}