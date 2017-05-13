using System;
using mPower.Domain.Accounting.Enums;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Account_AggregationStatus_UpdatedEvent : Event
    {
        public AggregatedAccountStatusEnum NewStatus { get; set; }
        public string AccountId { get; set; }
        public decimal IntuitAccountId { get; set; }
        public string LedgerId { get; set; }
        public string AggregationExceptionId { get; set; }
        public DateTime Date { get; set; }
    }
}