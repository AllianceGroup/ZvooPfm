using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_Questions_SetCommandHandler : IMessageHandler<CreditIdentity_Questions_SetCommand>
    {
        private readonly IRepository _repository;

        public CreditIdentity_Questions_SetCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreditIdentity_Questions_SetCommand message)
        {
            var creditIdentity = _repository.GetById<CreditIdentityAR>(message.ClientKey);
            creditIdentity.SetCommandMetadata(message.Metadata);
            creditIdentity.SetQuestions(message.Questions);

            _repository.Save(creditIdentity);
        }
    }
}