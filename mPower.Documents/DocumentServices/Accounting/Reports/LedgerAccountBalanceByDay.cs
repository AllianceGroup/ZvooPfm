using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.DocumentServices.Accounting.Reports
{
    public class LedgerAccountBalanceByDay
    {
        public LedgerAccountBalanceByDay()
        {
            AmountPerDay = new List<DateAmount>();
            SubAccounts = new List<LedgerAccountBalanceByDay>();
        }

        public string AccountId { get; set; }

        public string Name { get; set; }

        public List<DateAmount> AmountPerDay { get; set; }

        public AccountTypeEnum AccountType { get; set; }

        public List<LedgerAccountBalanceByDay> SubAccounts { get; set; }

        public int Order { get; set; }
    }
}
