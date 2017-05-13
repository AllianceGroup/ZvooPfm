using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Ledger.Data;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Account_UpdatedOrderEvent : Event
    {
        public string LedgerId { get; set; }

        public List<AccountOrderData> Orders { get; set; }
    }
}