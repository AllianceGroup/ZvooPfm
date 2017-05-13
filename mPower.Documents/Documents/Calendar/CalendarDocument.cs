using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.Enums;
using System.Collections.Generic;

namespace mPower.Documents.Documents.Calendar
{
    public class CalendarDocument
    {
        public CalendarDocument()
        {
            CalendarEvents = new List<CalendarEventDocument>();
            RepeatingEvents = new List<CalendarRepeatingEventDocument>();
        }

        [BsonId]
        public string Id { get; set; }

        public string LedgerId { get; set; }
        public string Name { get; set; }
        public CalendarTypeEnum Type { get; set; }
        public List<CalendarEventDocument> CalendarEvents { get; set; }
        public List<CalendarRepeatingEventDocument> RepeatingEvents { get; set; }
    }
}
