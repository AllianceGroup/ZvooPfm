using System;
using mPower.Domain.Accounting.Calendar.Data;

namespace mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts
{
    public class CalendarEventAlertDocument : DashboardAlertDocument
    {
        public string CalendarEventId { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public SendAlertOption SendAlertOptions { get; set; }
    }
}