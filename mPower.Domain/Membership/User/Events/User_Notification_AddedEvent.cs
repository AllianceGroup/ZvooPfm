using System;
using Paralect.Domain;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Membership.User.Events
{
    [Obsolete]
    public class User_Notification_AddedEvent : Event
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public EmailTypeEnum Type { get; set; }

        public string ClientKey { get; set; }

        public long AmountInCents { get; set; }

        public bool Enabled { get; set; }

        public bool SendEmail { get; set; }

        public bool SendText { get; set; }
    }
}