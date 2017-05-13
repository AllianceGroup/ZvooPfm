using System;
using System.Collections.Generic;
using System.Web.Mvc;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Calendar.Data
{
    public class SendAlertOption
    {
        public AlertModeEnum Mode { get; set; }
        public int Count { get; set; }
        public SendAlertTimeRange TimeRange { get; set; }
        [Obsolete]
        public IEnumerable<SelectListItem> Modes { get; set; }
    }
}
