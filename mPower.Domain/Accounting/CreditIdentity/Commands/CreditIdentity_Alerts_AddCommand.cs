using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.CreditIdentity.Data;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_Alerts_AddCommand : Command
    {
        public string ClientKey { get; set; }

        public List<AlertData> Alerts { get; set; }
    }
}