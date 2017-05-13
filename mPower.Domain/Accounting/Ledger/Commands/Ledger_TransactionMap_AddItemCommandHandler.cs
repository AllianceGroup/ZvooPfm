using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_TransactionMap_AddItemCommandHandler : IMessageHandler<Ledger_TransactionMap_AddItemCommand>
    {
        private readonly IRepository _repository;

        public Ledger_TransactionMap_AddItemCommandHandler(IRepository repository)
        {
            _repository = repository;
        }
        
        public void Handle(Ledger_TransactionMap_AddItemCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);
            ledger.SetCommandMetadata(message.Metadata);
            ledger.AddToItemTransactionMap(message.Keyword, message.AccountId);
            _repository.Save(ledger);
        }
    }
}