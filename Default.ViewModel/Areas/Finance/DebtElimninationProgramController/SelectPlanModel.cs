using mPower.Domain.Accounting.DebtElimination.Data;
using mPower.Domain.Accounting.Enums;
using System.Collections.Generic;

namespace Default.ViewModel.Areas.Finance.DebtElimninationProgramController
{
    public class SelectPlanModel
    {
        public SelectPlanModel()
        {
            MaxLoans = new List<MaxLoan>();
        }
        public DebtEliminationPlanEnum Plan { get; set; }
        public decimal MonthlyBudget { get; set; }
        public int YearsUntilRetirement { get; set; }
        public float EstimatedInvestmentEarningsRate { get; set; }
        public decimal AmountToSavings { get; set; }// Debt Elimination Changes
        public decimal LumpSumAmount { get; set; }// Debt Elimination Changes
        public int CurrentDebtMonth { get; set; }
        public double NewLoanAmount { get; set; }
        public decimal LoanInterestRate { get; set; }
        public List<MaxLoan> MaxLoans { get; set; }
        public decimal CurrentSavingsTotal { get; set; }
        public decimal CurrentDeathBenefit { get; set; }
        public int DeathBenefitTerminatesAge { get; set; }
        public decimal MonthlySavingsContribution { get; set; }
        public int Term1 { get; set; }
        public int Term2 { get; set; }
        public decimal Term1Amount { get; set; }
        public decimal Term2Amount { get; set; }
        public bool MonthlyContributionFBS { get; set; }
        public bool BudgetForFBS { get; set; }
    }
}