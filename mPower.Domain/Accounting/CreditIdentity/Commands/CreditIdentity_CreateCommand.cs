using Paralect.Domain;
using mPower.Domain.Accounting.CreditIdentity.Data;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_CreateCommand : Command
    {
        public string UserId { get; set; }

        public CreditIdentityData Data { get; set; }

        public CreditIdentity_CreateCommand()
        {
            Data = new CreditIdentityData();
        }
    }
}