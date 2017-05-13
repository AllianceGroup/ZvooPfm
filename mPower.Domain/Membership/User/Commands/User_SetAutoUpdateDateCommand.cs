using System;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_SetAutoUpdateDateCommand: Command
    {
        public string UserId { get; set; }

        public DateTime Date { get; set; }
    }
}