using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Budget_SetCommandHandler : IMessageHandler<Ledger_Budget_SetCommand>
    {
        private readonly IRepository _repository;

        public Ledger_Budget_SetCommandHandler(IRepository repository)
        {
            _repository = repository;
        }


        public void Handle(Ledger_Budget_SetCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);

            ledger.SetCommandMetadata(message.Metadata);
            ledger.SetBudgets(message.Budgets);

            _repository.Save(ledger);
        }
    }
}
