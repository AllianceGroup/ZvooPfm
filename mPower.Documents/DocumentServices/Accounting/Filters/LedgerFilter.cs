using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Accounting.Filters
{
    public class LedgerFilter : BaseFilter
    {
        public string AccountId { get; set; }
        public string UserId { get; set; }
        public AccountFilter AccountFilter { get; set; }
    }

    public class AccountFilter
    {
        public DateTime? AggregationStartedDateLessThan { get; set; }

        public List<AggregatedAccountStatusEnum> AggregationsStatusIn { get; set; }
    }
}
