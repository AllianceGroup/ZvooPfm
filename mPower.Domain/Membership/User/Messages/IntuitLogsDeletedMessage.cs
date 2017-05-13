using Paralect.Domain;

namespace mPower.Domain.Membership.User.Messages
{
    public class IntuitLogsDeletedMessage : Event
    {
        public string UserId { get; set; }
    }
}