using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Accounting.Filters
{
    public class TransactionsStatisticFilter : BaseFilter
    {
        public string LedgerId { get; set; }

        public string AccountId { get; set; }

        public string ParentAccountId { get; set; }

        public List<string> AccountIds { get; set; }

        public AccountTypeEnum? AccountType { get; set; }

        public Nullable<int> Year { get; set; }

        public int? Month { get; set; }

        public string AccountName { get; set; }
    }
}
