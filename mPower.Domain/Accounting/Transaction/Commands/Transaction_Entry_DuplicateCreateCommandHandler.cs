using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_Entry_DuplicateCreateCommandHandler : IMessageHandler<Transaction_Entry_DuplicateCreateCommand>
    {
        private readonly IRepository _repository;

        public Transaction_Entry_DuplicateCreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Transaction_Entry_DuplicateCreateCommand message)
        {
            var ledger = _repository.GetById<TransactionAR>(message.DuplicateId);
            ledger.SetCommandMetadata(message.Metadata);
            ledger.CreateDuplicateEntry(message.DuplicateId, message.LedgerId, message.ManualEntry, message.PotentialDuplicates);

            _repository.Save(ledger);
        }
    }
}
