using Paralect.Domain;

namespace mPower.Domain.Accounting.Goal.Events
{
    public class Goal_MarkedAsCompletedEvent : Event
    {
        public string GoalId { get; set; }
    }
}