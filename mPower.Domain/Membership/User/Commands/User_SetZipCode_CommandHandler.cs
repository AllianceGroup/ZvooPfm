using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_SetZipCode_CommandHandler: IMessageHandler<User_SetZipCode_Command>
    {
        private readonly IRepository _repository;

        public User_SetZipCode_CommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_SetZipCode_Command message)
        {
            var ar = _repository.GetById<UserAR>(message.UserId);
            ar.SetCommandMetadata(message.Metadata);
            ar.SetZipCode(message.ZipCode);
            _repository.Save(ar);
        }
    }
}