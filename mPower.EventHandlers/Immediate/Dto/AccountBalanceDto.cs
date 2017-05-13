using System;
using mPower.Domain.Accounting.Enums;

namespace mPower.EventHandlers.Immediate.Dto
{
    public class AccountBalanceDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public long BalanceDelta { get; set; }

        public long DebitAmountInCents { get; set; }

        public long CreditAmountInCents { get; set; }

        public AccountTypeEnum AccountType { get; set; }

        public DateTime Date { get; set; }
    }
}
