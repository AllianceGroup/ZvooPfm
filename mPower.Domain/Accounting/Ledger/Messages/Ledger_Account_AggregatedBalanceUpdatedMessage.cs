using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Messages
{
    public class Ledger_Account_AggregatedBalanceUpdatedMessage : Event
    {
        public String UserId { get; set; }
        public String LedgerId { get; set; }
        public String AccountId { get; set; }
        public String AccountName { get; set; }
        public AccountLabelEnum LabelEnum { get; set; }
        public Int64 OldValueInCents { get; set; }
        public Int64 NewBalance { get; set; }
        public DateTime Date { get; set; }
        public long CreditLimitInCents { get; set; }
    }
}