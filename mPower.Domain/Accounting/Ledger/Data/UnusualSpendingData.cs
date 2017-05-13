namespace mPower.Domain.Accounting.Ledger.Data
{
    public class UnusualSpendingData
    {
        public string UserId { get; set; }
        public string AccountName { get; set; }
        public long MonthlyAmountInCents { get; set; }
        public long AverageAmountInCents { get; set; } 
    }
}