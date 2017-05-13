using Paralect.Domain;

namespace mPower.Domain.Accounting.DebtElimination.Events
{
    public class DebtElimination_DebtToIncomeRatio_UpdatedEvent : Event
    {
        public string Id { get; set; }

        public long MonthlyGrossIncomeInCents { get; set; }

        public long TotalMonthlyRentInCents { get; set; }

        public long TotalMonthlyPitiaInCents { get; set; }

        public long TotalMonthlyDebtInCents { get; set; }

        public double DebtToIncomeRatio { get; set; }

        public string DebtToIncomeRatioString { get; set; }
    }
}
