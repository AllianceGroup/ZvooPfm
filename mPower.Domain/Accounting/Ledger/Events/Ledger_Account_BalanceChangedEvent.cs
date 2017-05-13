using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Account_BalanceChangedEvent : Event
    {
        public String UserId { get; set; }
        public String LedgerId { get; set; }
        public String AccountId { get; set; }
        public String AccountName { get; set; }
        public AccountLabelEnum AccountLabel { get; set; }
        public Int64 OldValueInCents { get; set; }
        public Int64 BalanceInCents { get; set; }
        public DateTime Date { get; set; }
    }
}