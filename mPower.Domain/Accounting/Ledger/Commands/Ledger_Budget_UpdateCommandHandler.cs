using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Budget_UpdateCommandHandler : IMessageHandler<Ledger_Budget_UpdateCommand>
    {
        private readonly IRepository _repository;

        public Ledger_Budget_UpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Ledger_Budget_UpdateCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);

            ledger.SetCommandMetadata(message.Metadata);
            ledger.UpdateBudget(message.BudgetId, message.AmountInCents, message.Month, message.Year);

            _repository.Save(ledger);
        }
    }
}
