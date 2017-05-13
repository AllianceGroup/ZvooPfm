using Paralect.Domain;

namespace mPower.Domain.Accounting.Calendar.Events
{
    public class Calendar_RepeatingCalendarEventDeletedEvent : Event
    {
        public string CalendarId { get; set; }
        public string EventId { get; set; }
    }
}
