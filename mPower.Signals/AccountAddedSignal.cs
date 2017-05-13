using Paralect.Domain;

namespace mPower.Signals
{
    public class AccountAddedSignal : Event
    {
        public string UserId { get; set; }
    }
}
