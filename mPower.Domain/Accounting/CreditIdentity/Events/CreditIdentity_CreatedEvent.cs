using Paralect.Domain;
using mPower.Domain.Accounting.CreditIdentity.Data;

namespace mPower.Domain.Accounting.CreditIdentity.Events
{
    public class CreditIdentity_CreatedEvent : Event
    {
        public string UserId { get; set; }

        public CreditIdentityData Data { get; set; }
    }
}