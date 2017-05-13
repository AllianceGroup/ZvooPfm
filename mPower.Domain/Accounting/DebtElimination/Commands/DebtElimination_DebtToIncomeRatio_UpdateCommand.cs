using Paralect.Domain;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_DebtToIncomeRatio_UpdateCommand : Command
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
