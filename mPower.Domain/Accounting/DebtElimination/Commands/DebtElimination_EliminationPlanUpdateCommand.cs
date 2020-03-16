using Paralect.Domain;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.DebtElimination.Data;
using System.Collections.Generic;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_EliminationPlanUpdateCommand : Command
    {

        public DebtElimination_EliminationPlanUpdateCommand()
        {
            MaxLoans = new List<MaxLoan>();
        }
        public string Id { get; set; }

        public DebtEliminationPlanEnum PlanId { get; set; }

        public long MonthlyBudgetInCents { get; set; }
        public int YearsUntilRetirement { get; set; }
        public float EstimatedInvestmentEarningsRate { get; set; }
        public long AmountToSavings { get; set; }// Debt Elimination Changes
        public long LumpSumAmount { get; set; }// Debt Elimination Changes
        public int CurrentDebtMonth { get; set; }
        public double NewLoanAmount { get; set; }
        public decimal LoanInterestRate { get; set; }
        public List<MaxLoan> MaxLoans { get; set; }
        public long CurrentSavingsTotal { get; set; }
        public long CurrentDeathBenefit { get; set; }
        public int DeathBenefitTerminatesAge { get; set; }
        public long MonthlySavingsContribution { get; set; }
        public int Term1 { get; set; }
        public int Term2 { get; set; }
        public long Term1Amount { get; set; }
        public long Term2Amount { get; set; }
        public bool MonthlyContributionFBS { get; set; }
        public bool BudgetForFBS { get; set; }
    }
}
