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

        public void UpdateEliminationPlan(DebtEliminationPlanEnum planId, long budget)
        {
            Apply(new DebtElimination_EliminationPlanUpdatedEvent
            {
                Id = _id,
                MonthlyBudgetInCents = budget,
                PlanId = planId
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
