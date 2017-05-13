using Paralect.Domain;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_DeleteCommand : Command
    {
        public string CalendarId { get; set; }
    }
}
