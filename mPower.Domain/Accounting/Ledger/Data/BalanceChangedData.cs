using System;

namespace mPower.Domain.Accounting.Ledger.Data
{
    public class BalanceChangedData
    {
        public String UserId { get; set; }
        public String LedgerId { get; set; }
        public String AccountId { get; set; }
        public String AccountName { get; set; }
        public Int64 OldValueInCents { get; set; }
        public Int64 BalanceInCents { get; set; }
        public DateTime Date { get; set; }
    }
}