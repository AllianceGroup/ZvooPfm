using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_DisplayMode_UpdateCommand : Command
    {
        public string Id { get; set; }

        public DebtEliminationDisplayModeEnum DisplayMode { get; set; }
    }
}