using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.DebtElimination.Data;
using mPower.Domain.Accounting.DebtElimination.Events;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;

namespace mPower.Domain.Accounting.DebtElimination
{
    public class DebtEliminationAR : MpowerAggregateRoot
    {
        /// <summary>
        /// For object reconstraction
        /// </summary>
        public DebtEliminationAR() { }

        public DebtEliminationAR(string id, string userId, string ledgerId, ICommandMetadata metadata)
        {
            SetCommandMetadata(metadata);

            Apply(new DebtElimination_CreatedEvent
            {
                Id = id,
                LedgerId = ledgerId,
                UserId = userId
            });
        }

        public void SetDebts(List<DebtItemData> debts)
        {
            Apply(new DebtElimination_Debts_SetEvent
            {
                DebtEliminationId = _id,
                Debts = debts,
            });
        }

        public void UpdateEliminationPlan(DebtEliminationPlanEnum planId, long budget, float estimatedInvestmentEarningsRate,int yearsUntilRetirement, long AmountToSavings,
            long LumpSumAmount, double NewLoanAmount, int CurrentDebtMonth, decimal LoanInterestRate, List<MaxLoan> MaxLoans, long CurrentSavingsTotal, long CurrentDeathBenefit,
            int DeathBenefitTerminatesAge ,long MonthlySavingsContribution, int Term1, int Term2, long Term1Amount, long Term2Amount
            , bool MonthlyContibutionFBS, bool BudgetFBS)
        {
            Apply(new DebtElimination_EliminationPlanUpdatedEvent
            {
                Id = _id,
                MonthlyBudgetInCents = budget,
                PlanId = planId,
                EstimatedInvestmentEarningsRate= estimatedInvestmentEarningsRate,
                YearsUntilRetirement= yearsUntilRetirement,
                AmountToSavings=AmountToSavings,// Debt Elimination Changes
                LumpSumAmount = LumpSumAmount,// Debt Elimination Changes
                NewLoanAmount=NewLoanAmount,// Debt Elimination Changes
                CurrentDebtMonth = CurrentDebtMonth,// Debt Elimination Changes
                LoanInterestRate= LoanInterestRate,// Debt Elimination Changes
                MaxLoans= MaxLoans,// Debt Elimination Changes
               CurrentSavingsTotal=CurrentSavingsTotal,
               CurrentDeathBenefit=CurrentDeathBenefit,
               DeathBenefitTerminatesAge=DeathBenefitTerminatesAge,
               MonthlySavingsContribution=MonthlySavingsContribution,
               Term1=Term1,
               Term2=Term2,
               Term1Amount=Term1Amount,
               Term2Amount=Term2Amount,
               MonthlyContributionFBS=MonthlyContibutionFBS,
               BudgetForFBS=BudgetFBS
            });
        }

        public void UpdateDisplayMode(DebtEliminationDisplayModeEnum displayMode)
        {
            Apply(new DebtElimination_DisplayMode_UpdatedEvent
            {
                Id = _id,
                DisplayMode = displayMode,
            });
        }

        public void AddEliminationToCalendar(string calendarId)
        {
            Apply(new DebtElimination_AddedToCalendarEvent
            {
                DebtEliminationId = _id,
                CalendarId = calendarId,
            });
        }

        public void AddMortgageProgram(MortgageProgramData data)
        {
            Apply(new DebtElimination_MortgageProgram_AddedEvent
                      {
                          Id = data.Id,
                          DebtEliminationId = _id,
                          Title = data.Title,
                          LoanAmountInCents = data.LoanAmountInCents,
                          InterestRatePerYear = data.InterestRatePerYear,
                          LoanTermInYears = data.LoanTermInYears,
                          PaymentPeriod = data.PaymentPeriod,
                          ExtraPaymentInCentsPerPeriod = data.ExtraPaymentInCentsPerPeriod,
                          DisplayResolution = data.DisplayResolution,
                      });
        }

        public void AddMortgageProgramToCalendar(string mortgageProgramId, string calendarId)
        {
            Apply(new DebtElimination_MortgageProgram_AddedToCalendarEvent
                      {
                          DebtEliminationId = _id,
                          MortgageProgramId = mortgageProgramId,
                          CalendarId = calendarId,
                      });
        }

        public void UpdateMortgageProgram(MortgageProgramData data)
        {
            Apply(new DebtElimination_MortgageProgram_UpdatedEvent
                      {
                          Id = data.Id,
                          DebtEliminationId = _id,
                          Title = data.Title,
                          LoanAmountInCents = data.LoanAmountInCents,
                          InterestRatePerYear = data.InterestRatePerYear,
                          LoanTermInYears = data.LoanTermInYears,
                          PaymentPeriod = data.PaymentPeriod,
                          ExtraPaymentInCentsPerPeriod = data.ExtraPaymentInCentsPerPeriod,
                          DisplayResolution = data.DisplayResolution,
                          AddedToCalendar = data.AddedToCalendar,
                      });
        }

        public void DeleteMortgageProgram(string programId, string calendarId)
        {
            Apply(new DebtElimination_MortgageProgram_DeletedEvent
                      {
                          Id = programId,
                          DebtEliminationId = _id,
                          CalendarId = calendarId,
                      });
        }

        public void UpdateDebtToIncomeRatio(long monthlyGrossIncomeInCents, long totalMonthlyRentInCents,
            long totalMonthlyPitiaInCents, long totalMonthlyDebtInCents, double debtToIncomeRatio, string dtiString)
        {
            Apply(new DebtElimination_DebtToIncomeRatio_UpdatedEvent
            {
                DebtToIncomeRatio = debtToIncomeRatio,
                DebtToIncomeRatioString = dtiString,
                Id = _id,
                MonthlyGrossIncomeInCents = monthlyGrossIncomeInCents,
                TotalMonthlyDebtInCents = totalMonthlyDebtInCents,
                TotalMonthlyPitiaInCents = totalMonthlyPitiaInCents,
                TotalMonthlyRentInCents = totalMonthlyRentInCents
            });
        }

        #region Object Reconstruction

        protected void On(DebtElimination_CreatedEvent created)
        {
            _id = created.Id;
        }


        #endregion
    }
}
