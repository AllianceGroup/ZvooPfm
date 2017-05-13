using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_ChangeNameCommand : Command
    {
        public String LedgerId { get; set; }
        public String AccountId { get; set; }
        public String Name { get; set; }
    }
}