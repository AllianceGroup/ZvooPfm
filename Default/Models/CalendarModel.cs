using System.Collections.Generic;
using System.Collections;
using System.Web.Mvc;
using mPower.Documents.Documents.Calendar;

namespace Default.Models
{
    public class CalendarModel 
    {
        public string SelectedCalendarId { get; set; }
        public IEnumerable<CalendarEventModel> Events { get; set; }
        public string LedgerId { get; set; }
        public IEnumerable Calendars { get; set; }

        public CalendarModel()
        {
            Events = new List<CalendarEventModel>();
            Calendars = new List<SelectListItem>();
        }

    }
}