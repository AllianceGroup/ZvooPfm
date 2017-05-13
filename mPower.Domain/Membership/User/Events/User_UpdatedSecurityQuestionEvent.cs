using Paralect.Domain;

namespace mPower.Domain.Membership.User.Events
{
    public class User_UpdatedSecurityQuestionEvent : Event
    {
        public string UserId { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }
    }
}
