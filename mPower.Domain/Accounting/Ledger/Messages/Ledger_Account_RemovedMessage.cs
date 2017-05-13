using System;
using mPower.Domain.Accounting.Enums;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Messages
{
    public class Ledger_Account_RemovedMessage : Event
    {
        public string UserId { get; set; }
        public string LedgerId { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public AccountLabelEnum LabelEnum { get; set; }
        public long Balance { get; set; }
        public long CreditLimitInCents { get; set; }
        public long? IntuitAccountId { get; set; }
        public DateTime Date { get; set; }
        public bool IsAggregated { get; set; }
    }
}
