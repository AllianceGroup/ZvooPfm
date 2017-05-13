using System;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_MobileLoggedInEvent : Event
    {
        public string UserId { get; set; }

        public DateTime Date { get; set; }

        public string AccessToken { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string AffiliateName { get; set; }

        public string AffiliateId { get; set; }
    }
}
