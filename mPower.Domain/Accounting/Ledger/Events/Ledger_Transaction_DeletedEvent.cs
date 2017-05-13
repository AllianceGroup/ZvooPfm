using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Transaction_DeletedEvent : Event
    {
        public string LedgerId { get; set; }
        public string TransactionId { get; set; }
    }
}