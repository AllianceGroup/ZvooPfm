using System;

namespace mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts
{
    public class DashboardAlertDocument : BaseNotification
    {
        public DateTime CreatedDate { get; set; }

        public string LedgerId { get; set; }

        public string Text { get; set; }
    }
}
