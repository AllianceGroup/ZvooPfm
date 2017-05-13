using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Calendar.Data;

namespace mPower.Domain.Accounting.Calendar.Events
{
    public class Calendar_OnetimeCalendarEventChangeStatusEvent : Event
    {
        public string CalendarId { get; set; }
        public string EventId { get; set; }
        public bool NewStatus { get; set; }

        // additional data
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        [Obsolete]
        public dynamic Type { get; set; }
        public SendAlertOption SendAlertOptions { get; set; }
        public string ParentId { get; set; }
    }
}
