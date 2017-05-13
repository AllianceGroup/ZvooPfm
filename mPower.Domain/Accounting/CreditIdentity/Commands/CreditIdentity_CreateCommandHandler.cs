using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_CreateCommandHandler : IMessageHandler<CreditIdentity_CreateCommand>
    {
        private readonly IRepository _repository;

        public CreditIdentity_CreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreditIdentity_CreateCommand message)
        {
            var creditIdentity = new CreditIdentityAR(message.UserId, message.Data, message.Metadata);

            _repository.Save(creditIdentity);
        }
    }
}