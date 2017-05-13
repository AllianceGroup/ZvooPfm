using Paralect.Domain;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Events
{
    public class User_Realestate_AddedEvent : Event
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public long AmountInCents { get; set; }

        public RealestateRawData RawData { get; set; }
    }
}