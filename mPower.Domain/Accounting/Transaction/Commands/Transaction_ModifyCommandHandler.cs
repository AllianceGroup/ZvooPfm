using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_ModifyCommandHandler : IMessageHandler<Transaction_ModifyCommand>
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Transaction_ModifyCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Transaction_ModifyCommand message)
        {
            var transactionAr = _repository.GetById<TransactionAR>(message.TransactionId);
            transactionAr.SetCommandMetadata(message.Metadata);

            transactionAr.ModifyTransaction(
                message.LedgerId,
                message.Type,
                message.Entries,
                message.BaseEntryAccountId,
                message.BaseEntryAccountType,
                message.ReferenceNumber,
                message.Imported);

            _repository.Save(transactionAr);
        }
    }
}