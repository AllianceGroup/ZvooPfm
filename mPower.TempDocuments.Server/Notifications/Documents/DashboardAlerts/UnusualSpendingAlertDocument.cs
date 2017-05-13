namespace mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts
{
    public class UnusualSpendingAlertDocument : DashboardAlertDocument
    {
        public string AccountName { get; set; }
        public long MonthlyAmountInCents { get; set; }
        public long AverageAmountInCents { get; set; }
    }
}