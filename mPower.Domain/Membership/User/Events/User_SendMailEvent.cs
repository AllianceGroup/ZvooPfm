using System;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    [Obsolete("Use 'mPower.TempEvents.SendMailMessage' instead.")]
    public class User_SendMailEvent : Event
    {
        public string UserId { get; set; }
        public string AffiliateId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}
