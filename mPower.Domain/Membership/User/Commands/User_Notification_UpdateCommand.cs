using Paralect.Domain;
using mPower.Domain.Application.Enums;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Notification_UpdateCommand : Command
    {
        public string UserId { get; set; }

        public EmailTypeEnum Type { get; set; }

        public bool? SendEmail { get; set; }

        public bool? SendText { get; set; }

        public int? BorderValue { get; set; }
    }
}
