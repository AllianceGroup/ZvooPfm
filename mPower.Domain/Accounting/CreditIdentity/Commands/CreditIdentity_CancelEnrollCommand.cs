using Paralect.Domain;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_CancelEnrollCommand : Command
    {
        public string CreditIdentityId { get; set; }
    }
}
