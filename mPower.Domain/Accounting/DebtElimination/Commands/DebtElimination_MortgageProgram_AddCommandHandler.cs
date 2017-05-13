using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.DebtElimination.Data;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_MortgageProgram_AddCommandHandler : IMessageHandler<DebtElimination_MortgageProgram_AddCommand>
    {
        private readonly IRepository _repository;

        public DebtElimination_MortgageProgram_AddCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(DebtElimination_MortgageProgram_AddCommand message)
        {
            var debtEliminationAr = _repository.GetById<DebtEliminationAR>(message.DebtEliminationId);
            debtEliminationAr.SetCommandMetadata(message.Metadata);
            var data = new MortgageProgramData
                           {
                               Id = message.Id,
                               Title = message.Title,
                               LoanAmountInCents = message.LoanAmountInCents,
                               InterestRatePerYear = message.InterestRatePerYear,
                               LoanTermInYears = message.LoanTermInYears,
                               PaymentPeriod = message.PaymentPeriod,
                               ExtraPaymentInCentsPerPeriod = message.ExtraPaymentInCentsPerPeriod,
                               DisplayResolution = message.DisplayResolution,
                           };
            debtEliminationAr.AddMortgageProgram(data);

            _repository.Save(debtEliminationAr);
        }
    }
}