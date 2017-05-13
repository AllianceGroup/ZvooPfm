using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.Ledger.Data;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_CreateCommandHandler : IMessageHandler<Ledger_Account_CreateCommand>
    {
        private readonly IRepository _repository;

        public Ledger_Account_CreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Ledger_Account_CreateCommand message)
        {
            var ledger = _repository.GetById<LedgerAR>(message.LedgerId);
            ledger.SetCommandMetadata(message.Metadata);

            ledger.CreateAccount(message.AccountId, new AccountData
            {
                Name = message.Name, 
                TypeEnum = message.AccountTypeEnum, 
                LabelEnum = message.AccountLabelEnum, 
                Imported = message.Imported,
                Aggregated = message.Aggregated,
                IntuitInstitutionId = message.IntuitInstitutionId,
                IntuitAccountId = message.IntuitAccountId,
                Description = message.Description,
                Number = message.Number,
                ParentAccountId = message.ParentAccountId,
                InterestRatePerc = message.InterestRatePerc,
                MinMonthPaymentInCents = message.MinMonthPaymentInCents,
                CreditLimitInCents = message.CreditLimitInCents,
                InstitutionName = message.InstitutionName,
                IntuitAccountNumber = message.IntuitAccountNumber,
                IntuitCategoriesNames = message.IntuitCategoriesNames,
            });

            _repository.Save(ledger);
        }
    }
}

