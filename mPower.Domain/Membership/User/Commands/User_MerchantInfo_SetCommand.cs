using Paralect.Domain;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_MerchantInfo_SetCommand : Command
    {
        public string UserId { get; set; }
        public MerchantData MerchantInfo { get; set; }
    }
}