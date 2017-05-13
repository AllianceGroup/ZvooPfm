using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_UpdateOrderCommandHandler : IMessageHandler<Ledger_Account_UpdateOrderCommand>
    {
        private readonly IRepository _repository;

        public Ledger_Account_UpdateOrderCommandHandler(IRepository repository)
        {
            _repository = repository;
        }


        public void Handle(Ledger_Account_UpdateOrderCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);

            ledger.SetCommandMetadata(message.Metadata);
            ledger.UpdateAccountsOrder(message.Orders);

            _repository.Save(ledger);
        }
    }
}
