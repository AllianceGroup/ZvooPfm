using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_AggregationStatus_UpdateCommand : Command
    {
        public AggregatedAccountStatusEnum NewStatus { get; set; }
        public string AccountId { get; set; }
        public long IntuitAccountId { get; set; }
        public string LedgerId { get; set; }
        public string ExceptionId { get; set; }
        public DateTime Date { get; set; }
    }
}