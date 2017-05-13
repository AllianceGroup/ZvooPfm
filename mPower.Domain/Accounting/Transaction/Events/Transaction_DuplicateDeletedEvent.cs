using Paralect.Domain;

namespace mPower.Domain.Accounting.Transaction.Events
{
    public class Transaction_DuplicateDeletedEvent : Event
    {
        public string Id { get; set; }
        public string LedgerId { get; set; }
    }
}