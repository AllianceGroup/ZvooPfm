using System;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_ChangePasswordCommand : Command
    {
        public string UserId { get; set; }

        public string PasswordHash { get; set; }

        public DateTime ChangeDate { get; set; }
    }
}
