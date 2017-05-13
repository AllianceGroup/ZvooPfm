using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.Ledger.Data;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_AggregatedBalanceUpdateCommandHandler : IMessageHandler<Ledger_Account_AggregatedBalanceUpdateCommand>
    {
        private readonly IRepository _repository;

        public Ledger_Account_AggregatedBalanceUpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Ledger_Account_AggregatedBalanceUpdateCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);

            ledger.SetCommandMetadata(message.Metadata);
            var data = new BalanceChangedData
                           {
                               UserId = message.UserId,
                               LedgerId = message.LedgerId,
                               AccountId = message.AccountId,
                               AccountName = message.AccountName,
                               OldValueInCents = message.OldValueInCents,
                               BalanceInCents = message.BalanceInCents,
                               Date = message.Date,
                           };
            ledger.UpdateAccountAggregatedBalance(data);

            _repository.Save(ledger);
        }
    }
}
