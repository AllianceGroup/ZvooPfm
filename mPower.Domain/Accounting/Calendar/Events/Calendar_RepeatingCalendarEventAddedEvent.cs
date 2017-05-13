using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Calendar.Events
{
    [Obsolete]
    public class Calendar_RepeatingCalendarEventAddedEvent : Event
    {
        public string UserId { get; set; }
        public string CalendarId { get; set; }
        public string EventId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        [Obsolete]
        public dynamic Type { get; set; }
        public dynamic SendAlertOptions { get; set; }
        public DayOfWeek[] RepeatOn { get; set; }
        public CalendarEventFrequencyEnum Frequency { get; set; }
        public DayAsPartOfEnum DayAsPartOf { get; set; }
        public int Period { get; set; }
        public DateTime StartDate { get; set; }
        public RepeatingEventEndOption End { get; set; }
    }
}
