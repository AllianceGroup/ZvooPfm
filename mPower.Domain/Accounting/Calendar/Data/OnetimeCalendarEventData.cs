using System;

namespace mPower.Domain.Accounting.Calendar.Data
{
    public class OnetimeCalendarEventData
    {
        public string UserId { get; set; }
        public string EventId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        public SendAlertOption SendAlertOptions { get; set; }
        public string ParentId { get; set; }
        public bool IsDone { get; set; }
    }
}
