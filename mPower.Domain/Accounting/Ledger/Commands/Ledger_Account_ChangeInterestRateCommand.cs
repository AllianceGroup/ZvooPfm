using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_ChangeInterestRateCommand: Command
    {
        public string LedgerId { get; set; }

        public string AccountId { get; set; }

        public float InterestRatePerc { get; set; }
    }

    public class Ledger_Account_ChangeInterestRateCommandHandler : IMessageHandler<Ledger_Account_ChangeInterestRateCommand>
    {
        private readonly IRepository _repository;

        public Ledger_Account_ChangeInterestRateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Ledger_Account_ChangeInterestRateCommand message)
        {
            var ar = _repository.GetById<LedgerAR>(message.LedgerId);
            ar.SetCommandMetadata(message.Metadata);
            ar.ChangeAccountInterestRate(message.AccountId, message.InterestRatePerc);
            _repository.Save(ar);
        }
    }
}