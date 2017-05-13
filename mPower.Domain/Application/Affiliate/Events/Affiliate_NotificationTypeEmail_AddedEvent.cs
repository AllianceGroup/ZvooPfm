using System;
using Paralect.Domain;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Application.Affiliate.Events
{
    public class Affiliate_NotificationTypeEmail_AddedEvent : Event
    {
        public string AffiliateId { get; set; }

        public string Name { get; set; }

        public EmailTypeEnum EmailType { get; set; }

        public string EmailContentId { get; set; }

        public TriggerStatusEnum Status { get; set; }

        [Obsolete]
        public NotificationScheduleEnum Schedule { get; set; }

        [Obsolete]
        public int Hour { get; set; }

        [Obsolete]
        public int Minute { get; set; }
    }
}