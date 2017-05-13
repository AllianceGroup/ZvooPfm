using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Accounting.Filters
{
    public class BudgetFilter : BaseFilter
    {
        public string LedgerId { get; set; }

        public string AccountId { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }

        /// <summary>
        /// Year * 12 + month
        /// </summary>
        public int? YearPlusMonthFrom { get; set; }

        /// <summary>
        /// Year * 12 + month
        /// </summary>
        public int? YearPlusMonthTo { get; set; }

        public AccountTypeEnum? AccountType { get; set; }
    }
}
