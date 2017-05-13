using Paralect.Domain;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Realestate_AddCommand : Command
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public long AmountInCents { get; set; }

        public RealestateRawData RawData { get; set; }
    }
}