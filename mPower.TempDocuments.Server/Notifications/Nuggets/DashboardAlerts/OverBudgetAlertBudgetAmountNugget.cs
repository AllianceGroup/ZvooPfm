using System;
using System.Collections.Generic;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;

namespace mPower.TempDocuments.Server.Notifications.Nuggets.DashboardAlerts
{
    public class OverBudgetAlertBudgetAmountNugget : INugget
    {
        public string Tag
        {
            get { return "budgetAmount"; }
        }

        public string DisplayName
        {
            get { return "Budget Amount"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return new List<EmailTypeEnum> {EmailTypeEnum.OverBudget}; }
        }

        public string TestValue
        {
            get { return AccountingFormatter.ConvertToDollarsThenFormat(50000); }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            var result = String.Empty;

            var alert = notification as OverBudgetAlertDocument;
            if (alert != null)
            {
                return AccountingFormatter.ConvertToDollarsThenFormat(alert.BudgetAmount);
            }

            return result;
        }
    }
}
