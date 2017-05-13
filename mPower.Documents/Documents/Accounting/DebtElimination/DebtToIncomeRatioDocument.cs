
namespace mPower.Documents.Documents.Accounting.DebtElimination
{
    public class DebtToIncomeRatioDocument
    {
        public long MonthlyGrossIncomeInCents { get; set; }

        public long TotalMonthlyRentInCents { get; set; }

        public long TotalMonthlyPitiaInCents { get; set; }

        public long TotalMonthlyDebtInCents { get; set; }

        public double DebtToIncomeRatio { get; set; }

        public string DebtToIncomeRatioString { get; set; }
    }
}
