using System;

namespace Default.Models
{
    public class CalendarEventModel
    {
        public string Id { get; set; }
        public string CalendarId { get; set; }
        public string ParentId { get; set; }
        public bool IsDone { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string DeleteAction { get; set; }
    }
}