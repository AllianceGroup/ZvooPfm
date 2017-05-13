namespace Default.ViewModel.Areas.Credit.ReportCard
{
    public class ReportCard
    {
        public double OpenCreditCardUtilizationRatio { get; set; }
        public string OpenCreditCardUtilizationGrade { get; set; }
        public double PercentOfPaymentsOnTimeRatio { get; set; }
        public string PercentOfPaymentsOnTimeGrade { get; set; }
        public string AvgAge { get; set; }
        public string AvgAgeGrade { get; set; }
        public int TotalAccounts { get; set; }
        public string TotalAccountsGrade { get; set; }
        public int InqueriesCount { get; set; }
        public string InqueriesGrade { get; set; }
        public int OtherNegativeFactorsCount { get; set; }
        public string OtherNegativeFactorsGrade { get; set; }
        public string Grade { get; set; }

        //For the Total Accounts paragraph
        public int OpenAccountsCount        { get; set; }
        public int ClosedAccountsCount      { get; set; }
        public int RevolvingAccountsCount   { get; set; }
        public int InstallmentAccountsCount { get; set; }
        public int MortgageAccountsCount    { get; set; }
        public int OtherAccountsCount       { get; set; }
    }
}
