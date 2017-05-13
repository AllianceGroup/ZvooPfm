using System;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Calendar
{
    public class CalendarEventDocument
    {
        [BsonId]
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string CalendarId { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public SendAlertOption SendAlertOptions { get; set; }
        public bool IsDone { get; set; }
    }

    public class CalendarRepeatingEventDocument : CalendarEventDocument
    {
        public DayOfWeek[] RepeatOn { get; set; }
        public CalendarEventFrequencyEnum Frequency { get; set; }
        public DayAsPartOfEnum DayAsPartOf { get; set; }
        public int Period { get; set; }
        public DateTime StartDate { get; set; }
        public RepeatingEventEndOption End { get; set; }

        public CalendarRepeatingEventDocument()
        {
            Frequency = CalendarEventFrequencyEnum.Weekly;
            DayAsPartOf = DayAsPartOfEnum.Month;
        }
    }
}
