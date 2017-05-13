using Paralect.Domain;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_RepeatingCalendarEvent_MarkDoneCommand : Command
    {
        public string CalendarId { get; set; }
        public string EventId { get; set; }
    }
}
