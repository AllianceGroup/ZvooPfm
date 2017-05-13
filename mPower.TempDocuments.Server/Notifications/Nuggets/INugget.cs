using System.Collections.Generic;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.Documents.Documents.Membership;

namespace mPower.TempDocuments.Server.Notifications.Nuggets
{
    public interface INugget
    {
        string Tag { get; }

        string DisplayName { get; }

        List<EmailTypeEnum> AcceptableEmails { get; }

        string TestValue { get; }

        string GetValue(UserDocument user, BaseNotification notification);
    }
}
