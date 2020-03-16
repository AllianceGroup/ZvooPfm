namespace mPower.Domain.Accounting.DebtElimination.Data
{
    public class DebtItemData
    {
        public string DebtId { get; set; }

        public string Name { get; set; }

        public decimal BalanceInCents { get; set; }

        public float InterestRatePerc { get; set; }

        public decimal MinMonthPaymentInCents { get; set; }

        public decimal ActualPayment { get; set; }// Debt Elimination Changes
    }

    // Debt Elimination Changes
    public class MaxLoan
    {
        public int Year { get; set; }

        public double CashSurrenderValue { get; set; }

        public double OriginalDeathBenefit { get; set; }

        public double NetDeathBenefit { get; set; }

        public double CurrentLoanWithInterest { get; set; }

        public double MaxNewLoan { get; set; }
    }
}