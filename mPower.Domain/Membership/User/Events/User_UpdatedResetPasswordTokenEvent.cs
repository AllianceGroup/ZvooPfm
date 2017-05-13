using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_UpdatedResetPasswordTokenEvent : Event
    {
        public string UserId { get; set; }

        public string UniqueToken { get; set; }
    }
}
