using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Domain.Application.Enums;
using mPower.Documents.Documents.Membership;
using mPower.TempDocuments.Server.Notifications.Documents;

namespace mPower.TempDocuments.Server.Notifications.Nuggets.User
{
    public class UserEmailNugget : INugget
    {
        public string Tag
        {
            get { return "email"; }
        }

        public string DisplayName
        {
            get { return "Email"; }
        }

        public List<EmailTypeEnum> AcceptableEmails
        {
            get { return Enum.GetValues(typeof (EmailTypeEnum)).Cast<EmailTypeEnum>().ToList(); }
        }

        public string TestValue
        {
            get { return "some@email.com"; }
        }

        public string GetValue(UserDocument user, BaseNotification notification)
        {
            return user.Email;
        }

    }
}
