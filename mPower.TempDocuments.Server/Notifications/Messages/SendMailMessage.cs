using System.Collections.Generic;
using Paralect.Domain;

namespace mPower.TempDocuments.Server.Notifications.Messages
{
    public class SendMailMessage : Event
    {
        public List<string> UsersIds { get; set; }
        public string AffiliateId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }

        public SendMailMessage()
        {
            IsBodyHtml = true;
        }
    }
}