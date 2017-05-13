using System;
using System.Collections.Generic;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;
using mPower.TempDocuments.Server.Notifications.Documents.Triggers;

namespace mPower.TempDocuments.Server.Notifications.Nuggets
{
    public class AccountNameNugget : INugget
    {
        public string Tag
        {
            get { return "accountName"; }
        }

        public string DisplayName
        {
            get { return "Account Name"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get
            {
                return new List<EmailTypeEnum>
                {
                    EmailTypeEnum.LowBalance,
                    EmailTypeEnum.LargePurchases,
                    EmailTypeEnum.AvailableCredit,
                    EmailTypeEnum.UnusualSpending,
                    EmailTypeEnum.OverBudget,
                    EmailTypeEnum.NewAccountAggregation
                };
            }
        }

        public string TestValue
        {
            get { return "My first account"; }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            var result = String.Empty;

            switch (notification.Type)
            {
                case EmailTypeEnum.LowBalance:
                    var lowBalanceNotification = notification as LowBalanceAlertDocument;
                    if (lowBalanceNotification != null)
                        result = lowBalanceNotification.AccountName;
                    break;
                case EmailTypeEnum.LargePurchases:
                    var largePurchasesNotification = notification as LargePurchaseAlertDocument;
                    if (largePurchasesNotification != null)
                        result = largePurchasesNotification.AccountName;
                    break;
                case EmailTypeEnum.AvailableCredit:
                    var availableCreditNotification = notification as AvailableCreditAlertDocument;
                    if (availableCreditNotification != null)
                        result = availableCreditNotification.AccountName;
                    break;
                case EmailTypeEnum.UnusualSpending:
                    var unusualSpendingNotification = notification as UnusualSpendingAlertDocument;
                    if (unusualSpendingNotification != null)
                        result = unusualSpendingNotification.AccountName;
                    break;
                case EmailTypeEnum.OverBudget:
                    var overBudgetNotification = notification as OverBudgetAlertDocument;
                    if (overBudgetNotification != null)
                        result = overBudgetNotification.AccountName;
                    break;
                case EmailTypeEnum.NewAccountAggregation:
                    var aggregateAccount = notification as NewAggregatedAccountTriggerNotification;
                    if (aggregateAccount != null)
                        result = aggregateAccount.AccountName;
                    break;
            }

            return result;
        }
    }
}
