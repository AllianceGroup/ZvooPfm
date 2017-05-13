using Paralect.Domain;

namespace mPower.Domain.Accounting.DebtElimination.Events
{
    public class DebtElimination_AddedToCalendarEvent : Event
    {
        public string DebtEliminationId { get; set; }

        public string CalendarId { get; set; }
    }
}