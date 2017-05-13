namespace mPower.Domain.Accounting.DebtElimination.Data
{
    public class DebtItemData
    {
        public string DebtId { get; set; }

        public string Name { get; set; }

        public long BalanceInCents { get; set; }

        public float InterestRatePerc { get; set; }

        public long MinMonthPaymentInCents { get; set; }
    }
}