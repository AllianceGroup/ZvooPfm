using Paralect.Domain;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_MortgageProgram_DeleteCommand : Command
    {
        public string Id { get; set; }

        public string DebtEliminationId { get; set; }

        public string CalendarId { get; set; }
    }
}