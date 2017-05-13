using System;
using System.Collections.Generic;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;

namespace mPower.TempDocuments.Server.Notifications.Nuggets.DashboardAlerts
{
    public class UnusualSpendingAlertAmountNugget : INugget
    {
        public string Tag
        {
            get { return "amount"; }
        }

        public string DisplayName
        {
            get { return "Over Usual Amount"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return new List<EmailTypeEnum> {EmailTypeEnum.UnusualSpending}; }
        }

        public string TestValue
        {
            get { return AccountingFormatter.ConvertToDollarsThenFormat(15000); }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            var result = String.Empty;

            var alert = notification as UnusualSpendingAlertDocument;
            if (alert != null)
                result = AccountingFormatter.ConvertToDollarsThenFormat(alert.MonthlyAmountInCents - alert.AverageAmountInCents);

            return result;
        }
    }
}
