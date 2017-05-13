using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Calendar.Data;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_OnetimeCalendarEvent_CreateCommand : Command
    {
        public string UserId { get; set; }
        public string CalendarId { get; set; }
        public string ParentId { get; set; }
        public string CalendarEventId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        public SendAlertOption SendAlertOptions { get; set; }
        public bool IsDone { get; set; }
    }
}
