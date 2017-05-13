using System;
using System.Collections.Generic;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;

namespace mPower.TempDocuments.Server.Notifications.Nuggets.DashboardAlerts
{
    public class OverBudgetAlertMonthNugget : INugget
    {
        public string Tag
        {
            get { return "month"; }
        }

        public string DisplayName
        {
            get { return "Budget Month"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return new List<EmailTypeEnum> {EmailTypeEnum.OverBudget}; }
        }

        public string TestValue
        {
            get { return GetValue(null, new OverBudgetAlertDocument {Month = DateTime.Now.Month}); }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            var result = String.Empty;

            var alert = notification as OverBudgetAlertDocument;
            if (alert != null)
            {
                return new DateTime(DateTime.Now.Year, alert.Month, 1).ToString("MMMM");
            }

            return result;
        }
    }
}
