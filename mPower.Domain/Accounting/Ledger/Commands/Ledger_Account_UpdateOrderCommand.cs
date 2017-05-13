using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Ledger.Data;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_UpdateOrderCommand : Command
    {
        public string LedgerId { get; set; }

        public List<AccountOrderData> Orders { get; set; }
    }
}
