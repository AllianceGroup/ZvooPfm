using System.Collections.Generic;
using Paralect.Domain;

namespace mPower.TempDocuments.Server.Notifications.Messages
{
    public class EmailManuallyCreatedMessage : Event
    {
        public List<string> UsersIds { get; set; }

        public string AffiliateId { get; set; }

        public string EmailContentId { get; set; }
    }
}