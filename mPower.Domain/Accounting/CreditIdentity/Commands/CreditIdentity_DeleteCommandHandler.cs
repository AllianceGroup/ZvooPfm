using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_DeleteCommandHandler : IMessageHandler<CreditIdentity_DeleteCommand>
    {
        private readonly IRepository _repository;

        public CreditIdentity_DeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreditIdentity_DeleteCommand message)
        {
            var creditIdentity = _repository.GetById<CreditIdentityAR>(message.ClientKey);

            creditIdentity.SetCommandMetadata(message.Metadata);

            creditIdentity.Delete();

            _repository.Save(creditIdentity);
        }
    }
}