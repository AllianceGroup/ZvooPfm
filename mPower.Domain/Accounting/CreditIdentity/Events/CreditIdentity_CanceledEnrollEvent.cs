using Paralect.Domain;

namespace mPower.Domain.Accounting.CreditIdentity.Events
{
    public class CreditIdentity_CanceledEnrollEvent : Event
    {
        public string CreditIdentityId { get; set; }
    }
}
