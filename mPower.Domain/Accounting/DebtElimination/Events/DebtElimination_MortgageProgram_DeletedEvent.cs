using Paralect.Domain;

namespace mPower.Domain.Accounting.DebtElimination.Events
{
    public class DebtElimination_MortgageProgram_DeletedEvent : Event
    {
        public string Id { get; set; }

        public string DebtEliminationId { get; set; }

        public string CalendarId { get; set; }
    }
}