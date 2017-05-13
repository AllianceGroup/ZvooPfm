using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_DeleteCommand : Command
    {
        public String LedgerId { get; set; }
    }
}