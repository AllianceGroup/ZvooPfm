using System;
using mPower.Domain.Accounting.Enums;

namespace mPower.EventHandlers.Eventual.Segments
{
    public class BalanceChangedDto
    {
        public String LedgerId { get; set; }
        public String AccountId { get; set; }
        public String AccountName { get; set; }
        public AccountLabelEnum LabelEnum { get; set; }
        public Int64 OldValueInCents { get; set; }
        public Int64 BalanceInCents { get; set; }
        public DateTime Date { get; set; }
        public long CreditLimitInCents { get; set; }
    }
}