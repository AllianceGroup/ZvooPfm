namespace mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts
{
    public class LargePurchaseAlertDocument : DashboardAlertDocument
    {
        public string AccountName { get; set; }
        public long PurchaseInCents { get; set; }
    }
}