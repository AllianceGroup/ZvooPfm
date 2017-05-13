using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Merchant_BillingInfo_SetCommandHandler : IMessageHandler<User_Merchant_BillingInfo_SetCommand>
    {
        private readonly IRepository _repository;

        public User_Merchant_BillingInfo_SetCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_Merchant_BillingInfo_SetCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.SetMerchantBillingInfo(message.BillingInfo);

            _repository.Save(user);
        }
    }
}