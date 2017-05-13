using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_RemoveCommand : Command
    {
        public String LedgerId { get; set; }
        public String AccountId { get; set; }

        public AccountLabelEnum? Label { get; set; }
    }
}