using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.Ledger.Data;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_UpdateCommandHandler : IMessageHandler<Ledger_Account_UpdateCommand>
    {
        private readonly IRepository _repository;

        public Ledger_Account_UpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Ledger_Account_UpdateCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);

            ledger.SetCommandMetadata(message.Metadata);
            ledger.UpdateAccount(message.AccountId, new AccountData
            {
                Name = message.Name,
                InstitutionName = message.InstitutionName,
                Description = message.Description,
                Number = message.Number,
                ParentAccountId = message.ParentAccountId,
                InterestRatePerc = message.InterestRatePerc,
                MinMonthPaymentInCents = message.MinMonthPaymentInCents,
                CreditLimitInCents = message.CreditLimitInCents
            });

            _repository.Save(ledger);
        }
    }
}

