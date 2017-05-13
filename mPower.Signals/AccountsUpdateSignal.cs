using Paralect.Domain;

namespace mPower.Signals
{
    public class AccountsUpdateSignal: Event
    {
        public string LedgerId { get; set; }
        public string UserId { get; set; }
    }
}
