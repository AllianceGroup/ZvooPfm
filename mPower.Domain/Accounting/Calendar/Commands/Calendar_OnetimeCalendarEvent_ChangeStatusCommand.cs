using Paralect.Domain;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_OnetimeCalendarEvent_ChangeStatusCommand : Command
    {
        public string EventId { get; set; }
        public string CalendarId { get; set; }
        public bool IsDone { get; set; }
    }
}
