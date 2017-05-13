using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_CreateCommandHandler : IMessageHandler<Transaction_CreateCommand>
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Transaction_CreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Transaction_CreateCommand message)
        {
            
                var ledger = new TransactionAR(
                    message.TransactionId,
                    message.UserId,
                    message.LedgerId,
                    message.Type,
                    message.Entries,
                    message.BaseEntryAccountId,
                    message.BaseEntryAccountType,
                    message.Imported,
                    message.ImportedTransactionId,
                    message.Metadata,
                    message.ReferenceNumber);

                _repository.Save(ledger);
        }
    }
}