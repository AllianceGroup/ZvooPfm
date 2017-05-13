using System;
using mPower.Domain.Accounting.Calendar.Data;

namespace mPower.Domain.Accounting.Calendar
{
    public class CalendarEvent
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        public SendAlertOption SendAlertOptions { get; set; }
        public bool IsDone { get; set; }
    }
}