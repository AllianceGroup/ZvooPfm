using Paralect.Domain;

namespace mPower.Domain.Accounting.Transaction.Events
{
    public class Transaction_DeletedEvent : Event
    {
        public string LedgerId { get; set; }
        public string TransactionId { get; set; }
        public bool IsMultipleDelete { get; set; }
    }
}