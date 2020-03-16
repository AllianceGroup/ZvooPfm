using System.Collections.Generic;
using Default.ViewModel.Areas.Finance.DebtToolsController;
using mPower.Documents.Documents.Accounting.DebtElimination;
using mPower.Domain.Accounting.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using mPower.Domain.Accounting.DebtElimination.Data;
 

namespace mPower.WebApi.Tenants.ViewModels.DebtElimninationProgram
{
    public class Step3Model
    {
        public double PayoffTime { get; set; }
        public double PayoffTimeViaPlan { get; set; }

        public double TotalPayed { get; set; }
        public double TotalPayedViaPlan { get; set; }

        public double TotalPayedViaFBS { get; set; }

        public double PayoffTimeViaFBS { get; set; }

        public double MoneySaved => TotalPayed - TotalPayedViaPlan;

        public double InterestSaved => InterestPaid - InterestPaidViaPlan;
        public double InterestSavedFBS => InterestPaid - InterestPaidViaFBS;

        public double YearsSaved => PayoffTime - PayoffTimeViaPlan;

        public SelectList DisplayModes { get; set; }

        public ChartViewModel Chart { get; set; }

        public List<ChartItems> InterestPaidChart { get; set; }
        public List<ChartItems> DebtFreeChart { get; set; }

        public List<ProgramDetailsItemShort> Details { get; set; }

        public string ExtraAmount { get; set; }

        public bool AddedToCalendar { get; set; }

        public DebtEliminationDisplayModeEnum DisplayMode { get; set; }
       
        public Step3Model()
        {
            Details = new List<ProgramDetailsItemShort>();
            Debts = new List<DebtItemData>();
            ComparisonDebts = new List<DebtEliminationComparisionModel>();
            SavingsDebts = new List<DebtEliminationComparisionModel>();
            InterestPaidChart = new List<ChartItems>();
            DebtFreeChart = new List<ChartItems>();
        }
        public double TotalEstimatedInvestment { get; set; }
        public int YearsUntilRetirement { get; set; }
        public float EstimatedInvestmentEarningsRate { get; set; }

        public string ErrorMessage { get; set; }
        public long AmountToSavings { get; set; }// Debt Elimination Changes
        public long LumpSumAmount { get; set; }// Debt Elimination Changes
        public List<DebtItemData> Debts { get; set; }// Debt Elimination Changes
        public string PlanName { get; set; }// Debt Elimination Changes
        public List<DebtEliminationComparisionModel> SavingsDebts { get; set; }
        public List<DebtEliminationComparisionModel> ComparisonDebts { get; set; }// Debt Elimination Changes
        public DebtEliminationSavingsSummaryModel SavingSummary { get; set; }// Debt Elimination Changes
        public double InterestPaid { get; set; }// Debt Elimination Changes
        public double InterestPaidViaPlan { get; set; }// Debt Elimination Changes
        public double InterestPaidViaFBS { get; set; }
        public int CurrentDebtMonth { get; set; }
        public double NewLoanAmount { get; set; }

        public string LoanBalanceImageUrl { get; set; }
        public string DebtSummaryImageUrl { get; set; }
        public double DeathBenefit { get; set; }
        public double CurrentSavingsTotal { get; set; }
        public long CurrentDeathBenefit { get; set; }
        public long FBSDeathBenefit { get; set; }
        public int DeathBenefitTerminatesAge { get; set; }
        public long MonthlySavingsContribution { get; set; }
        public bool MonthlyContributionFBS { get; set; }
        public bool BudgetForFBS { get; set; }
        public double FBSSavings { get; set; }

        public double FinalSavings { get; set; }
        public double PayTime { get; set; }
        public double FinalInterestSaved { get; set; }

        public AFWPlanData AFWPlanData { get; set; }
        public FBSPlanData FBSPlanData { get; set; }

        public double CPPaymentSavings { get; set; }
    }

    public class ChartItems
    {
        public double value { get; set; }
        public string label { get; set; }
        public string color { get; set; }
        public ChartItems(double value, string label, string color)
        {
            this.value = value;
            this.label = label;
            this.color = color;
        }
    }

    public class AFWPlanData
    {
        public double TotalPayed { get; set; }
        public double PayOffTime { get; set; }
        public double MoneySaved { get; set; }
        public double InterestPaid { get; set; }
        public double InterestSaved { get; set; }
        public double YearsSaved { get; set; }
        public long  AmountToSavings { get; set; }
        public double TotalAmountToSavings { get; set; }
        public double MaxAmountToSavings { get; set; }
        public double TotalSavings { get; set; }
    }

    public class FBSPlanData
    {
        public double TotalPayed { get; set; }
        public double PayOffTime { get; set; }
        public double MoneySaved { get; set; }
        public double InterestPaid { get; set; }
        public double InterestSaved { get; set; }
        public double YearsSaved { get; set; }
        public long AmountToSavings { get; set; }
        public double TotalAmountToSavings { get; set; }
        public double MaxAmountToSavings { get; set; }
        public double TotalSavings { get; set; }
    }

}
