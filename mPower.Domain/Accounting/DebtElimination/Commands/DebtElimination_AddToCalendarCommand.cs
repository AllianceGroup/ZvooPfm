using Paralect.Domain;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_AddToCalendarCommand : Command
    {
        public string DebtEliminationId { get; set; }

        public string CalendarId { get; set; }
    }
}