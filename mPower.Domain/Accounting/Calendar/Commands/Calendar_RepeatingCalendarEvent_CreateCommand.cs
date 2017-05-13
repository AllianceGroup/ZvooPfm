using System;
using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_RepeatingCalendarEvent_CreateCommand : Command
    {
        public string UserId { get; set; }
        public string CalendarId { get; set; }
        public string EventId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        public SendAlertOption SendAlertOptions { get; set; }
        public IEnumerable<DayOfWeek> RepeatOn { get; set; }
        public CalendarEventFrequencyEnum Frequency { get; set; }
        public DayAsPartOfEnum DayAsPartOf { get; set; }
        public int Period { get; set; }
        public DateTime StartDate { get; set; }
        public RepeatingEventEndOption End { get; set; }
    }
}
