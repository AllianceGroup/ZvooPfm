using System;
using System.Collections.Generic;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;

namespace mPower.TempDocuments.Server.Notifications.Nuggets.DashboardAlerts
{
    public class AvailableCreditAlertAvailableCreditNugget : INugget
    {
        public string Tag
        {
            get { return "availableCredit"; }
        }

        public string DisplayName
        {
            get { return "Available Credit"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return new List<EmailTypeEnum> {EmailTypeEnum.AvailableCredit}; }
        }

        public string TestValue
        {
            get { return AccountingFormatter.ConvertToDollarsThenFormat(10000); }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            var result = String.Empty;

            var alert = notification as AvailableCreditAlertDocument;
            if (alert != null)
                result = AccountingFormatter.ConvertToDollarsThenFormat(alert.AvailableCreditInCents);

            return result;
        }
    }
}
