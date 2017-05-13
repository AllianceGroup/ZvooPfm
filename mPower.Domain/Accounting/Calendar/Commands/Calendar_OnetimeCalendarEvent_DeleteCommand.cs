using Paralect.Domain;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_OnetimeCalendarEvent_DeleteCommand : Command
    {
        public string CalendarId { get; set; }
        public string EventId { get; set; }
    }
}
