using mPower.Domain.Application.Enums;

namespace mPower.Domain.Membership.User.Data
{
    public class NotificationData
    {
        public EmailTypeEnum Type { get; set; }

        public bool? SendEmail { get; set; }

        public bool? SendText { get; set; }

        public int? BorderValue { get; set; }
    }
}