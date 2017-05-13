using System;
using Paralect.Domain;

namespace mPower.Domain.Membership.User.Messages
{
    public class User_CreatedMessage : Event
    {
        public string UserId { get; set; }
        
        public DateTime CreateDate { get; set; }
        
        public string AffiliateId { get; set; }
        
        public string AffiliateName { get; set; }

        public bool IsActive { get; set; }
    }
}