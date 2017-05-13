using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Account_RenamedEvent:Event
    {
        public string LedgerId { get; set; }

        public string AccountId { get; set; }

        public string Name { get; set; }
    }
}