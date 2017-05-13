using Paralect.Domain;

namespace mPower.Domain.Accounting.DebtElimination.Events
{
    public class DebtElimination_MortgageProgram_AddedToCalendarEvent : Event
    {
        public string DebtEliminationId { get; set; }

        public string MortgageProgramId { get; set; }

        public string CalendarId { get; set; }
    }
}