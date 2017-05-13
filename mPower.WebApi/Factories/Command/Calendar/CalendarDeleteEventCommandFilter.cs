using mPower.WebApi.Tenants.Model;

namespace mPower.WebApi.Factories.Command.Calendar
{
    public class CalendarDeleteEventCommandFilter
    {
        public string Calendarid { get; set; }
        public string EventId { get; set; }
        public CalendarEventTypeEnum Type { get; set; }
        public string ParentId { get; set; }
    }
}
