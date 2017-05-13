using System.Collections.Generic;
using mPower.Framework.Services;
using System;

namespace mPower.Documents.DocumentServices
{
    public class EventLogFilter : BaseFilter
    {
        public EventLogFilter()
        {
            ExcludeFields = new List<string>() { "Data" };
        }
        public string UserId { get; set; }

        public DateTime? MinStoredDate { get; set; }
    }
}
