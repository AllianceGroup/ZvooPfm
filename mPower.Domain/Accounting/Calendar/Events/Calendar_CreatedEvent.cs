using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Calendar.Events
{
    public class Calendar_CreatedEvent : Event
    {
        public string CalendarId { get; set; }
        public string LedgerId { get; set; }
        public string Name { get; set; }
        public CalendarTypeEnum Type { get; set; }
    }
}
