using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.CreditIdentity.Events
{
    public class CreditIdentity_MarkedAsVerifiedEvent : Event
    {
        public string ClientKey { get; set; }

        public DateTime Date { get; set; }

        public string IpAddress { get; set; }
    }
}