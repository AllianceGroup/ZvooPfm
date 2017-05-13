using Paralect.Domain;

namespace mPower.Domain.Accounting.Calendar.Events
{
    public class Calendar_DeletedEvent : Event
    {
        public string CalendarId { get; set; }
    }
}
