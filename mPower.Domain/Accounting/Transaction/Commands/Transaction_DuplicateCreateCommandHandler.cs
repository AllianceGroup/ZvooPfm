using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_DuplicateCreateCommandHandler : IMessageHandler<Transaction_DuplicateCreateCommand>
    {
        private readonly IRepository _repository;

        public Transaction_DuplicateCreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Transaction_DuplicateCreateCommand message)
        {
            var ledger = new TransactionAR(message.BaseTransaction, message.PotentialDuplicates, message.Metadata);

            // Get The Transaction
            // ProcessDuplicate

            _repository.Save(ledger);
        }
    }
}
