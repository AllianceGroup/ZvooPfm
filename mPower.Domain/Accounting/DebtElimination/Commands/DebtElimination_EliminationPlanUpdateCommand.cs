using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_EliminationPlanUpdateCommand : Command
    {
        public string Id { get; set; }

        public DebtEliminationPlanEnum PlanId { get; set; }

        public long MonthlyBudgetInCents { get; set; }
    }
}
