using Paralect.Domain;
using Paralect.ServiceBus;
using com.yodlee.soap.core.usermanagement;
using mPower.Domain.Yodlee.Services;

namespace mPower.Domain.Yodlee.YodleeUser.Commands
{
    public class YodleeUser_CancelCommandHandler : BaseYodleeService, IMessageHandler<YodleeUser_CancelCommand>
    {
        private readonly IRepository _repository;

        public YodleeUser_CancelCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(YodleeUser_CancelCommand message)
        {
            ConnectToYodlee();
            var user = LoginUser(message.Username, message.Password);

            var userRegistration = new UserRegistrationService();
            userRegistration.unregister(user.userContext);

        }
    }
}