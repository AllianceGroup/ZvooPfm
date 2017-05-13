using System;
using mPower.Domain.Accounting.Enums;

namespace Default.Models
{
    public class CalendarViewModelFilter
    {
        public string CalendarId { get; set; }
        public bool? IsDone { get; set; }
        public TimeSpanFilterEnum TimeSpanFilter { get; set; }
        public DateTime Date { get; set; }

        public CalendarViewModelFilter()
        {
            Date = DateTime.Now;
            TimeSpanFilter = TimeSpanFilterEnum.ByWeek;
        }
    }
}