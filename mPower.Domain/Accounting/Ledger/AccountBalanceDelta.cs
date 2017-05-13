using System;

namespace mPower.Domain.Accounting.Ledger
{
    public class AccountBalanceDelta
    {
        public String AccountId { get; set; }
        public Int64 CreditAmount { get; set; }
        public Int64 DebitAmount { get; set; }
    }
}