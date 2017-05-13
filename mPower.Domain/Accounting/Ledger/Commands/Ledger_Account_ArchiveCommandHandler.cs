using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_ArchiveCommandHandler : IMessageHandler<Ledger_Account_ArchiveCommand>
    {
        private readonly IRepository _repository;

        public Ledger_Account_ArchiveCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Ledger_Account_ArchiveCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);

            ledger.SetCommandMetadata(message.Metadata);
            ledger.ArchiveAccount(message.AccountId, message.Reason);

            _repository.Save(ledger);
        }
    }
}
