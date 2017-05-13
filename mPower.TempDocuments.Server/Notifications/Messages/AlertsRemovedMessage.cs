using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Application.Enums;

namespace mPower.TempDocuments.Server.Notifications.Messages
{
    public class AlertsRemovedMessage : Event
    {
        public List<string> Ids { get; set; }

        public EmailTypeEnum? Type { get; set; }

        public List<string> PublicKeys { get; set; }
    }
}