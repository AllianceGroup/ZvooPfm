using Paralect.Domain;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Merchant_BillingInfo_SetCommand : Command
    {
        public string UserId { get; set; }
        public BillingData BillingInfo { get; set; }
    }
}