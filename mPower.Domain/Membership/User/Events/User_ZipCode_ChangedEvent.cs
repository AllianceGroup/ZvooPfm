using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_ZipCode_ChangedEvent: Event
    {
        public string UserId { get; set; }
        public string ZipCode { get; set; }
    }
}