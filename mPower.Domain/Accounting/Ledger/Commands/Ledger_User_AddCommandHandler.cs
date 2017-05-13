using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_User_AddCommandHandler : IMessageHandler<Ledger_User_AddCommand>
    {
        private readonly Repository _repository;

        public Ledger_User_AddCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public string UserId { get; set; }

        public void Handle(Ledger_User_AddCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);
            ledger.SetCommandMetadata(message.Metadata);

            ledger.AddUser(message.UserId);

            _repository.Save(ledger);
        }
    }
}
