namespace mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts
{
    public class LowBalanceAlertDocument : DashboardAlertDocument
    {
        public string AccountName { get; set; }
        public long NewBalance { get; set; }
    }
}