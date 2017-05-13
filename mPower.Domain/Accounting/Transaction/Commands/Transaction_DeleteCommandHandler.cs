using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_DeleteCommandHandler : IMessageHandler<Transaction_DeleteCommand>
    {
        private readonly IRepository _repository;

        public Transaction_DeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Transaction_DeleteCommand message)
        {
            var transactionAr = _repository.GetById<TransactionAR>(message.TransactionId);

            transactionAr.SetCommandMetadata(message.Metadata);
            transactionAr.DeleteTransaction(message.LedgerId);

            _repository.Save(transactionAr);
        }
    }
}
