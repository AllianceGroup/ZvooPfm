using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Account_AggregatedBalanceUpdatedEvent : Event
    {
        public String UserId { get; set; }
        public String LedgerId { get; set; }
        public String AccountId { get; set; }
        public String AccountName { get; set; }
        public Int64 OldValueInCents { get; set; }
        public Int64 NewBalance { get; set; }
        public DateTime Date { get; set; }
    }
}