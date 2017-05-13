using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Calendar.Data;

namespace mPower.Domain.Accounting.Calendar.Events
{
    public class Calendar_OnetimeCalendarEventAddedEvent : Event
    {
        public string UserId { get; set; }
        public string CalendarId { get; set; }
        public string CalendarEventId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        [Obsolete]
        public dynamic Type { get; set; }
        public SendAlertOption SendAlertOptions { get; set; }
        public bool IsDone { get; set; }
        public string ParentId { get; set; }
    }
}
