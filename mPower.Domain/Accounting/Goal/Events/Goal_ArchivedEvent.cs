using Paralect.Domain;

namespace mPower.Domain.Accounting.Goal.Events
{
    public class Goal_ArchivedEvent : Event
    {
        public string GoalId { get; set; }
    }
}