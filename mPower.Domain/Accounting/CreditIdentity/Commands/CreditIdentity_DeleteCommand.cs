using Paralect.Domain;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_DeleteCommand : Command
    {
        public string ClientKey { get; set; }
    }
}