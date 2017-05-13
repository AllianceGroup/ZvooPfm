using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.DebtElimination.Events
{
    public class DebtElimination_DisplayMode_UpdatedEvent : Event
    {
        public string Id { get; set; }

        public DebtEliminationDisplayModeEnum DisplayMode { get; set; }
    }
}