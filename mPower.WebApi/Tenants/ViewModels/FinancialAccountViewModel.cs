namespace mPower.WebApi.Tenants.ViewModels
{
    public class FinancialAccountViewModel
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public long AmountInCents { get; set; }
        public string InstitutionName { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public float InterestRate { get; set; }
        public long? AvailableBalanceInCents { get; set; }
    }
}