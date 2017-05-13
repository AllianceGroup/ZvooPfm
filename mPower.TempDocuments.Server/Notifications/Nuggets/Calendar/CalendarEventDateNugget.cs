using System;
using System.Collections.Generic;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;

namespace mPower.TempDocuments.Server.Notifications.Nuggets.Calendar
{
    public class CalendarEventDateNugget : INugget
    {
        private const string DateFormat = "MMMM dd, yyyy h:mm tt";

        public string Tag
        {
            get { return "eventDate"; }
        }

        public string DisplayName
        {
            get { return "Calendar Event Date"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return new List<EmailTypeEnum> {EmailTypeEnum.CalendarEventCame}; }
        }

        public string TestValue
        {
            get { return DateTime.Now.AddMinutes(-5).ToString(DateFormat); }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            var result = String.Empty;

            var alert = notification as CalendarEventAlertDocument;
            if (alert != null)
            {
                return alert.Date.ToString(DateFormat);
            }

            return result;
        }
    }
}
