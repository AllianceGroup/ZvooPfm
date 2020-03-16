using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_EliminationPlanUpdateCommandHandler : IMessageHandler<DebtElimination_EliminationPlanUpdateCommand>
    {
        private readonly IRepository _repository;

        public DebtElimination_EliminationPlanUpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(DebtElimination_EliminationPlanUpdateCommand message)
        {
            var debtEliminationAr = _repository.GetById<DebtEliminationAR>(message.Id);

            debtEliminationAr.SetCommandMetadata(message.Metadata);
            debtEliminationAr.UpdateEliminationPlan(message.PlanId, message.MonthlyBudgetInCents, message.EstimatedInvestmentEarningsRate, message.YearsUntilRetirement,
                message.AmountToSavings, message.LumpSumAmount, message.NewLoanAmount, message.CurrentDebtMonth, message.LoanInterestRate, message.MaxLoans,
                message.CurrentSavingsTotal, message.CurrentDeathBenefit, message.DeathBenefitTerminatesAge, message.MonthlySavingsContribution,
                message.Term1, message.Term2, message.Term1Amount, message.Term2Amount
                , message.MonthlyContributionFBS, message.BudgetForFBS);

            _repository.Save(debtEliminationAr); 
        }
    }
}