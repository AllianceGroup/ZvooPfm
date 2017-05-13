using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_AggregationStatus_UpdateCommandHandler : IMessageHandler<Ledger_Account_AggregationStatus_UpdateCommand>
    {
        private readonly IRepository _repository;

        public Ledger_Account_AggregationStatus_UpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Ledger_Account_AggregationStatus_UpdateCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);

            ledger.SetCommandMetadata(message.Metadata);
            ledger.UpdateAccountAggregationStatus(message.AccountId, message.IntuitAccountId, message.NewStatus, message.Date, message.ExceptionId);

            _repository.Save(ledger);
        }
    }
}
