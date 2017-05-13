using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.DebtElimination.Events
{
    public class DebtElimination_EliminationPlanUpdatedEvent : Event
    {
        public string Id { get; set; }

        public DebtEliminationPlanEnum PlanId { get; set; }

        public long MonthlyBudgetInCents { get; set; }
    }
}
