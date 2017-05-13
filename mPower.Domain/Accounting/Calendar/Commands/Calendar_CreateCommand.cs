using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Calendar.Commands
{
    public class Calendar_CreateCommand : Command
    {
        public string CalendarId { get; set; }
        public string LedgerId { get; set; }
        public string Name { get; set; }
        public CalendarTypeEnum Type { get; set; }
    }
}
