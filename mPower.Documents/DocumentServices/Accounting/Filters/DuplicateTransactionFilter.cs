using System;
using System.Collections.Generic;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Accounting.Filters
{
    public class DuplicateTransactionFilter : BaseFilter
    {
        public string IdIsNot { get; set; }
        public string LedgerId { get; set; }
    }
}
