using System;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_PasswordChangedEvent : Event
    {
        public string UserId { get; set; }

        public string NewPassword { get; set; }

        public DateTime ChangeDate { get; set; }
    }
}
