using System;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_LogInCommand : Command
    {
        public string UserId { get; set; }

        public DateTime LogInDate { get; set; }

        public string AuthToken { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string AffiliateName { get; set; }

        public string AffiliateId { get; set; }
    }
}
