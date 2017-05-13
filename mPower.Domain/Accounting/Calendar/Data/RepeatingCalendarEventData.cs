using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Calendar.Data
{
    public class RepeatingCalendarEventData : OnetimeCalendarEventData
    {
        public IEnumerable<DayOfWeek> RepeatOn { get; set; }
        public CalendarEventFrequencyEnum Frequency { get; set; }
        public DayAsPartOfEnum DayAsPartOf { get; set; }
        public int Period { get; set; }
        public DateTime StartDate { get; set; }
        public RepeatingEventEndOption End { get; set; }

        public IEnumerable<OnetimeCalendarEventData> PrecalculatedData { get; set; }
    }
}
