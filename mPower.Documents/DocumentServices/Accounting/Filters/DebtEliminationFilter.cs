using System.Collections.Generic;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Accounting.Filters
{
    public class DebtEliminationFilter : BaseFilter
    {
        public string LedgerId { get; set; }
        public string UserId { get; set; }
    }
}
