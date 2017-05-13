using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.CreditIdentity.Events
{
    [Obsolete]
    public class CreditIdentity_Notification_SetStatusEvent : Event
    {
        public string ClientKey { get; set; }

        public bool Enabled { get; set; }
    }
}