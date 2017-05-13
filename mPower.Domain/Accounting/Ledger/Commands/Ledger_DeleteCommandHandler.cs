using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_DeleteCommandHandler : IMessageHandler<Ledger_DeleteCommand>
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Ledger_DeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Ledger_DeleteCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);
            ledger.SetCommandMetadata(message.Metadata);
            ledger.Delete();

            _repository.Save(ledger);
        }
    }
}