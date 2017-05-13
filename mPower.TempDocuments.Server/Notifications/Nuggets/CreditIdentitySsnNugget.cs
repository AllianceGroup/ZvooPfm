using System;
using System.Collections.Generic;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.Triggers;

namespace mPower.TempDocuments.Server.Notifications.Nuggets
{
    public class CreditIdentitySsnNugget : INugget
    {
        public string Tag
        {
            get { return "ssn"; }
        }

        public string DisplayName
        {
            get { return "Masked SSN"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return new List<EmailTypeEnum> {EmailTypeEnum.NewCreditIdentity}; }
        }

        public string TestValue
        {
            get { return "xxx-xx-1234"; }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            var result = String.Empty;

            var newCreditIdentityTriggerNotification = notification as NewCreditIdentityTriggerNotification;
            if (newCreditIdentityTriggerNotification != null)
                result = newCreditIdentityTriggerNotification.CreditIdentitySocial;

            return result;
        }
    }
}
