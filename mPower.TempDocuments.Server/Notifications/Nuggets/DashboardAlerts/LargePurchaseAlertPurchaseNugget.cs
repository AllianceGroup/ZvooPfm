using System;
using System.Collections.Generic;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;

namespace mPower.TempDocuments.Server.Notifications.Nuggets.DashboardAlerts
{
    public class LargePurchaseAlertPurchaseNugget : INugget
    {
        public string Tag
        {
            get { return "purchase"; }
        }

        public string DisplayName
        {
            get { return "Purchase"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return new List<EmailTypeEnum> {EmailTypeEnum.LargePurchases}; }
        }

        public string TestValue
        {
            get { return AccountingFormatter.ConvertToDollarsThenFormat(600000); }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            var result = String.Empty;

            var alert = notification as LargePurchaseAlertDocument;
            if (alert != null)
                result = AccountingFormatter.ConvertToDollarsThenFormat(alert.PurchaseInCents);

            return result;
        }
    }
}
