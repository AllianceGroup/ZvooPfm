using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_User_RemoveCommandHandler : IMessageHandler<Ledger_User_RemoveCommand>
    {
        private readonly Repository _repository;

        public Ledger_User_RemoveCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public string UserId { get; set; }

        public void Handle(Ledger_User_RemoveCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);
            ledger.SetCommandMetadata(message.Metadata);

            ledger.RemoveUser(message.UserId);

            _repository.Save(ledger);
        }
    }
}
