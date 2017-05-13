using Paralect.Domain;

namespace mPower.Domain.Accounting.Goal.Events
{
    public class Goal_DeletedEvent : Event
    {
        public string GoalId { get; set; }
    }
}