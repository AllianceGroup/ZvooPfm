using System;
using System.Collections.Generic;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;

namespace mPower.TempDocuments.Server.Notifications.Nuggets.Calendar
{
    public class CalendarEventDetailsNugget : INugget
    {
        public string Tag
        {
            get { return "eventDetails"; }
        }

        public string DisplayName
        {
            get { return "Calendar Event Details"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return new List<EmailTypeEnum> {EmailTypeEnum.CalendarEventCame}; }
        }

        public string TestValue
        {
            get { return "It's time to start new goal."; }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            var result = String.Empty;

            var alert = notification as CalendarEventAlertDocument;
            if (alert != null)
            {
                return alert.Description;
            }

            return result;
        }
    }
}
