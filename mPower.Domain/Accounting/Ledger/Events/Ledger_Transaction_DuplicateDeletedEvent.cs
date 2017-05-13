using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Transaction_DuplicateDeletedEvent : Event
    {
        public string Id { get; set; }
        public string LedgerId { get; set; }
    }
}