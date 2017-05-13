using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.DebtElimination.Data;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_MortgageProgram_UpdateCommandHandler : 
        IMessageHandler<DebtElimination_MortgageProgram_UpdateCommand>
    {
        private readonly IRepository _repository;

        public DebtElimination_MortgageProgram_UpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(DebtElimination_MortgageProgram_UpdateCommand message)
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
                               AddedToCalendar = message.AddedToCalendar,
                           };
            debtEliminationAr.UpdateMortgageProgram(data);

            _repository.Save(debtEliminationAr);
        }
    }
}