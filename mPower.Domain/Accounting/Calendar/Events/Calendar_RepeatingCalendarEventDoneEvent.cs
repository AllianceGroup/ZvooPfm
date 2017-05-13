using Paralect.Domain;

namespace mPower.Domain.Accounting.Calendar.Events
{
    public class Calendar_RepeatingCalendarEventDoneEvent : Event
    {
        public string CalendarId { get; set; }
        public string EventId { get; set; }
    }
}
