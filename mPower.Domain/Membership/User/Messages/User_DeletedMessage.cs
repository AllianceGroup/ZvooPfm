using Paralect.Domain;

namespace mPower.Domain.Membership.User.Messages
{
    public class User_DeletedMessage : Event
    {
        public string UserId { get; set; }
        public string AffiliateId { get; set; }
        public string AffiliateName { get; set; }
    }
}