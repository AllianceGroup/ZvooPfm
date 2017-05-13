using Paralect.Domain;

namespace mPower.Domain.Accounting.Goal.Commands
{
    public class Goal_DeleteCommand : Command
    {
        public string GoalId { get; set; }
    }
}