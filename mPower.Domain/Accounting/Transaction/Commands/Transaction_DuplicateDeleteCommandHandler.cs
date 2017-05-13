using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_DuplicateDeleteCommandHandler : IMessageHandler<Transaction_DuplicateDeleteCommand>
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Transaction_DuplicateDeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Transaction_DuplicateDeleteCommand message)
        {
            var ledger = _repository.GetById<TransactionAR>(message.Id);

            ledger.SetCommandMetadata(message.Metadata);
            ledger.DeleteDuplicate(message.LedgerId);

            _repository.Save(ledger);
        }
    }
}