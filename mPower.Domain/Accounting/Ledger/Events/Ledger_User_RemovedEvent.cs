using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_User_RemovedEvent : Event
    {
        public string LedgerId { get; set; }

        public string UserId { get; set; }
    }
}
