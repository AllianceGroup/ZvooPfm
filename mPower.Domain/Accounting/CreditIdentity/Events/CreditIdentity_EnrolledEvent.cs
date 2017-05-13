using Paralect.Domain;

namespace mPower.Domain.Accounting.CreditIdentity.Events
{
    public class CreditIdentity_EnrolledEvent : Event
    {
        public string CreditIdentityId { get; set; }

        public string MemberId { get; set; }

        public string ActivationCode { get; set; }

        public string SalesId { get; set; }
    }
}
