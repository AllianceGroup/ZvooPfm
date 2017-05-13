using Paralect.Domain;

namespace mPower.Domain.Accounting.Goal.Commands
{
    public class Goal_ArchiveCommand : Command
    {
        public string GoalId { get; set; }
    }
}