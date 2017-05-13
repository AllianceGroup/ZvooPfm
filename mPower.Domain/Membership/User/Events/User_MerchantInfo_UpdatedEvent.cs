using Paralect.Domain;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Events
{
    public class User_MerchantInfo_UpdatedEvent : Event
    {
        public string UserId { get; set; }
        public MerchantData MerchantInfo { get; set; }
    }
}