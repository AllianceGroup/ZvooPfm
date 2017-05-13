using System;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    [Obsolete]
    public class User_Notification_DeletedEvent : Event
    {
        public string UserId { get; set; }

        public string Id { get; set; }
    }
}