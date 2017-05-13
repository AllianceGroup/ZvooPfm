using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Transaction.Commands
{

    public class Transaction_Entry_ChangeAccountCommandHandler : IMessageHandler<Transaction_Entry_ChangeAccountCommand>
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Transaction_Entry_ChangeAccountCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Transaction_Entry_ChangeAccountCommand message)
        {
            var transactionAr = _repository.GetById<TransactionAR>(message.TransactionId);
            transactionAr.SetCommandMetadata(message.Metadata);

            foreach (var entry in message.Entries)
            {
                if (entry.AccountId == message.PreviousAccountId)
                    entry.AccountId = message.NewAccountId;
            }

            var baseAccountId = message.BaseEntryAccountId == message.PreviousAccountId
                                    ? message.NewAccountId
                                    : message.BaseEntryAccountId;

            transactionAr.ModifyTransaction(message.LedgerId,
                                            message.TransactionType,
                                            message.Entries,
                                            baseAccountId,
                                            message.BaseEntryAccountType,
                                            message.ReferenceNumber,
                                            message.IsTransactionImported);
            _repository.Save(transactionAr);
        }
    }

}
