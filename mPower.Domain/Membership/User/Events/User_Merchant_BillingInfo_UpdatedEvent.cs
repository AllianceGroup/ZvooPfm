using Paralect.Domain;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Events
{
    public class User_Merchant_BillingInfo_UpdatedEvent : Event
    {
        public string UserId { get; set; }
        public BillingData BillingInfo { get; set; }
    }
}