using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_Alerts_AddCommandHandler : IMessageHandler<CreditIdentity_Alerts_AddCommand>
    {
        private readonly IRepository _repository;

        public CreditIdentity_Alerts_AddCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreditIdentity_Alerts_AddCommand message)
        {
            var creditIdentity = _repository.GetById<CreditIdentityAR>(message.ClientKey);
            creditIdentity.SetCommandMetadata(message.Metadata);
            creditIdentity.AddAlerts(message.Alerts);

            _repository.Save(creditIdentity);
        }
    }
}