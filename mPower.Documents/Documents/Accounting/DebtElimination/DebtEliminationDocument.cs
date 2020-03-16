using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.DebtElimination.Data;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Accounting.DebtElimination
{
    public class DebtEliminationDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string LedgerId { get; set; }

        public DebtEliminationPlanEnum PlanId { get; set; }

        public long MonthlyBudgetInCents { get; set; }

        public DebtEliminationDisplayModeEnum DisplayMode { get; set; }

        public bool AddedToCalendar { get; set; }

        public List<MortgageAccelerationProgramDocument> MortgagePrograms { get; set; }

        public string CurrentMortgageProgramId { get; set; }

        public List<DebtItemData> Debts { get; set; }

        public DebtEliminationDocument()
        {
            DisplayMode = DebtEliminationDisplayModeEnum.ProgramAndMinimums;
            MortgagePrograms = new List<MortgageAccelerationProgramDocument>();
            DebtToIncomeRatio = new DebtToIncomeRatioDocument();
            Debts = new List<DebtItemData>();
            MaxLoans = new List<MaxLoan>();
        }

        public DebtToIncomeRatioDocument DebtToIncomeRatio { get; set; }

        public bool IsDebtToIncomeCalculatedBefore
        {
            get { return DebtToIncomeRatio.DebtToIncomeRatio > 0; }
        }

        public EstimatedInvestment EstimatedInvestments { get; set; }
        public double TotalEstimatedInvestment { get; set; }
        public float EstimatedInvestmentEarningsRate  { get; set; }
        public int YearsUntilRetirement { get; set; }

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
