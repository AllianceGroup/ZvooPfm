using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_AggregatedBalanceUpdateCommand : Command
    {
        public string AccountId { get; set; }
        public string LedgerId { get; set; }
        public String UserId { get; set; }
        public String AccountName { get; set; }
        public Int64 OldValueInCents { get; set; }
        public Int64 BalanceInCents { get; set; }
        public DateTime Date { get; set; }
    }
}