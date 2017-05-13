using System;
using System.Collections.Generic;

namespace Default.ViewModel.BusinessController.Reports
{
    public class BaseBusinessReportsModel
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public List<DateFormat> ReportDateFormats { get; set; }

        public string ReportDateFormatsJson { get; set; }

        public int CurrentReportDateFormat { get; set; }
    }
}
