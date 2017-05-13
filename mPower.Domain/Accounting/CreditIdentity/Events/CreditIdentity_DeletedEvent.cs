using Paralect.Domain;

namespace mPower.Domain.Accounting.CreditIdentity.Events
{
    public class CreditIdentity_DeletedEvent : Event
    {
        public string ClientKey { get; set; }
    }
}