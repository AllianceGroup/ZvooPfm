using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_MerchantInfo_SetCommandHandler : IMessageHandler<User_MerchantInfo_SetCommand>
    {
        private readonly IRepository _repository;

        public User_MerchantInfo_SetCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_MerchantInfo_SetCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.SetMerchantInfo(message.MerchantInfo);

            _repository.Save(user);
        }
    }
}