using Paralect.Domain;

namespace mPower.Domain.Accounting.DebtElimination.Events
{
    public class DebtElimination_CreatedEvent : Event
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string LedgerId { get; set; }
    }
}
