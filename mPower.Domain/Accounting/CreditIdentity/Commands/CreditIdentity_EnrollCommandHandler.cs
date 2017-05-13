using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_EnrollCommandHandler : IMessageHandler<CreditIdentity_EnrollCommand>
    {
        private readonly IRepository _repository;

        public CreditIdentity_EnrollCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreditIdentity_EnrollCommand message)
        {
            var creditIdentityAr = _repository.GetById<CreditIdentityAR>(message.CreditIdentityId);
            creditIdentityAr.SetCommandMetadata(message.Metadata);
            creditIdentityAr.Enroll(message.ActivationCode, message.MemberId, message.SalesId);
            _repository.Save(creditIdentityAr);
        }
    }
}
