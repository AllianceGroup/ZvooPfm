using System;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_AutoUpdateDateSetEvent:Event
    {
        public String UserId { get; set; }

        public DateTime Date { get; set; }
    }
}