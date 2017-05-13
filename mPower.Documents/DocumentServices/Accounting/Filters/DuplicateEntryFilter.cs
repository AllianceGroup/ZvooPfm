using System;
using System.Collections.Generic;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Accounting.Filters
{
    public class DuplicateEntryFilter : BaseFilter
    {
        public string LedgerId { get; set; }
    }
}
