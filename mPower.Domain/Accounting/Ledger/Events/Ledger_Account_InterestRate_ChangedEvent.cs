using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Account_InterestRate_ChangedEvent : Event
    {
        public string LedgerId { get; set; }

        public string AccountId { get; set; }

        public float InterestRatePerc { get; set; }
    }
}