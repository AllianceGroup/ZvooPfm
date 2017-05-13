using System;
using System.Collections.Generic;

namespace mPower.Domain.Accounting.Calendar
{
    public class CalendarEventCollection
    {
        public readonly Dictionary<string, CalendarEvent> EventsById = new Dictionary<string, CalendarEvent>();

        public Int32 Count
        {
            get { return EventsById.Count; }
        }

        public void Add(CalendarEvent calendarEvent)
        {
            EventsById[calendarEvent.Id] = calendarEvent;
        }

        public CalendarEvent Get(String calendarEventId)
        {
            return EventsById[calendarEventId];
        }

        public void Remove(String calendarEventId)
        {
            EventsById.Remove(calendarEventId);
        }

        public Boolean Exists(String calendarEventId)
        {
            return EventsById.ContainsKey(calendarEventId);
        }
    }
}