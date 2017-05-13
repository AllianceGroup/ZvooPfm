using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_MarkAsVerifiedCommandHandler : IMessageHandler<CreditIdentity_MarkAsVerifiedCommand>
    {
        private readonly IRepository _repository;

        public CreditIdentity_MarkAsVerifiedCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreditIdentity_MarkAsVerifiedCommand message)
        {
            var creditIdentity = _repository.GetById<CreditIdentityAR>(message.ClientKey);
            creditIdentity.SetCommandMetadata(message.Metadata);
            creditIdentity.MarkAsVerified(message.Date, message.IpAddress);

            _repository.Save(creditIdentity);
        }
    }
}