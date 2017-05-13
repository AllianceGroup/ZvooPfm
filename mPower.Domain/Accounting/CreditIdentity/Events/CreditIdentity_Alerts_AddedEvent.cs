using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.CreditIdentity.Data;

namespace mPower.Domain.Accounting.CreditIdentity.Events
{
    public class CreditIdentity_Alerts_AddedEvent : Event
    {
        public string ClientKey { get; set; }

        public List<AlertData> Alerts { get; set; }
    }
}