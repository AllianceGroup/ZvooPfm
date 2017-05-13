using System;
using System.Collections.Generic;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;

namespace mPower.TempDocuments.Server.Notifications.Nuggets.DashboardAlerts
{
    public class LowBalanceAlertNewBalanceNugget : INugget
    {
        public string Tag
        {
            get { return "newBalance"; }
        }

        public string DisplayName
        {
            get { return "New Balance"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return new List<EmailTypeEnum> {EmailTypeEnum.LowBalance}; }
        }

        public string TestValue
        {
            get { return AccountingFormatter.ConvertToDollarsThenFormat(100); }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            var result = String.Empty;

            var lowBalanceNotification = notification as LowBalanceAlertDocument;
            if (lowBalanceNotification != null)
                result = AccountingFormatter.ConvertToDollarsThenFormat(lowBalanceNotification.NewBalance);

            return result;
        }
    }
}
