using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.CreditIdentity.Commands
{
    public class CreditIdentity_MarkAsVerifiedCommand : Command
    {
        public string ClientKey { get; set; }

        public DateTime Date { get; set; }

        public string IpAddress { get; set; }
    }
}