using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Calendar.Data
{
    public class CalendarData
    {
        public CalendarTypeEnum Type { get; set; }

        public string LedgerId { get; set; }

        public string CalendarId { get; set; }
        
        public string Name { get; set; }

    }
}
