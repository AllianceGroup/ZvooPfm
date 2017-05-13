namespace mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts
{
    public class OverBudgetAlertDocument : DashboardAlertDocument
    {
        public string AccountName { get; set; }

        public int Month { get; set; }

        public long BudgetAmount { get; set; }

        public long SpentAmountWithSubAccounts { get; set; }
    }
}