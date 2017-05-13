using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Events
{
    public class Ledger_Account_RemovedEvent : Event
    {
        public String LedgerId { get; set; }
        public String AccountId { get; set; }

        public AccountLabelEnum? Label { get; set; }
    }
}