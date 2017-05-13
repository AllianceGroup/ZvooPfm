namespace mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts
{
    public class AvailableCreditAlertDocument : DashboardAlertDocument
    {
        public string AccountName { get; set; }
        public long SetAmountInCents { get; set; }
        public long AvailableCreditInCents { get; set; }
    }
}