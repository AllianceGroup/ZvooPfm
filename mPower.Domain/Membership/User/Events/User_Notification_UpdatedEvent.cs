using Paralect.Domain;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Membership.User.Events
{
    public class User_Notification_UpdatedEvent : Event
    {
        public string UserId { get; set; }

        public EmailTypeEnum Type { get; set; }

        public bool? SendEmail { get; set; }

        public bool? SendText { get; set; }

        public int? BorderValue { get; set; }
    }
}
