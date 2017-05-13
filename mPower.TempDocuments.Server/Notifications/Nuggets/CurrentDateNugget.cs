using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;

namespace mPower.TempDocuments.Server.Notifications.Nuggets
{
    public class CurrentDateNugget : INugget
    {
        private const string DateFormat = "MMMM dd, yyyy h:mm tt";

        public string Tag
        {
            get { return "date"; }
        }

        public string DisplayName
        {
            get { return "Current Date"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return Enum.GetValues(typeof (EmailTypeEnum)).Cast<EmailTypeEnum>().ToList(); }
        }

        public string TestValue
        {
            get { return DateTime.Now.ToString(DateFormat); }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            return DateTime.Now.ToString(DateFormat);
        }
    }
}
