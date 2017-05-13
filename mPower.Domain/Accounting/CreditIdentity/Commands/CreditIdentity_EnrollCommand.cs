using Paralect.Domain;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_EnrollCommand : Command
    {
        public string CreditIdentityId { get; set; }

        public string MemberId { get; set; }

        public string ActivationCode { get; set; }

        public string SalesId { get; set; }
    }
}
