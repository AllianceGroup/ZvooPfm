using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_CancelEnrollCommandHandler : IMessageHandler<CreditIdentity_CancelEnrollCommand>
    {
        private readonly IRepository _repository;

        public CreditIdentity_CancelEnrollCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreditIdentity_CancelEnrollCommand message)
        {
            var creditIdentityAr = _repository.GetById<CreditIdentityAR>(message.CreditIdentityId);
            creditIdentityAr.SetCommandMetadata(message.Metadata);
            creditIdentityAr.CancelEnroll();
            _repository.Save(creditIdentityAr);
        }
    }
}
