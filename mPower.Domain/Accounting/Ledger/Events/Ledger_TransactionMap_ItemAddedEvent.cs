using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_TransactionMap_ItemAddedEvent : Event
    {
        public string AccountId { get; set; }
        public string Keyword { get; set; }
        public string LedgerId { get; set; }
    }
}