using System.Linq;
using Paralect.Domain;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.Calendar.Events;
using mPower.Framework;

namespace mPower.Domain.Accounting.Calendar
{
    public class CalendarAR : MpowerAggregateRoot
    {
        public CalendarEventCollection Events;

        /// <summary>
        /// For object reconstraction
        /// </summary>
        public CalendarAR() { }

        public CalendarAR(CalendarData data, ICommandMetadata metadata)
        {
            SetCommandMetadata(metadata);

            Apply(new Calendar_CreatedEvent
            {
                Name = data.Name,
                CalendarId = data.CalendarId,
                LedgerId = data.LedgerId,
                Type = data.Type
            });
        }

        public void Delete()
        {
            Apply(new Calendar_DeletedEvent
            {
                CalendarId = _id
            });
        }

        public void AddOnetimeCalendarEvent(OnetimeCalendarEventData data)
        {
            Apply(new Calendar_OnetimeCalendarEventAddedEvent
            {
                UserId = data.UserId,
                CalendarId = _id,
                CalendarEventId = data.EventId,
                CreatedDate = data.CreatedDate,
                EventDate = data.EventDate,
                Description = data.Description,
                IsDone = data.IsDone, 
                SendAlertOptions = data.SendAlertOptions,
                ParentId = data.ParentId,
            });
        }

        public void AddRepeatingCalendarEvent(RepeatingCalendarEventData data)
        {
            Apply(new Calendar_RepeatingEventPrecalculated_AddedEvent
            {
                UserId = data.UserId,
                CalendarId = _id,
                Date = data.EventDate,
                EventId = data.EventId,
                Description = data.Description,
                SendAlertOptions = data.SendAlertOptions,
                End = data.End,
                Frequency = data.Frequency,
                DayAsPartOf = data.DayAsPartOf,
                Period = data.Period,
                RepeatOn = data.RepeatOn.ToArray(),
                StartDate = data.StartDate,
                PrecalculatedData = data.PrecalculatedData.ToList(),
            });

        }

        public void DeleteOnetimeCalendarEvent(string eventId)
        {
            Apply(new Calendar_OnetimeCalendarEventDeletedEvent
            {
                  CalendarId = _id,
                  EventId = eventId
            });
        }

        public void DeleteRepeatingCalendarEvent(string eventId)
        {
            Apply(new Calendar_RepeatingCalendarEventDeletedEvent
            {
                CalendarId = _id,
                EventId = eventId
            });
        }

        public void ChangeOnetimeCalendarEventStatus(string eventId, bool newStatus)
        {
            if (Events.Exists(eventId))
            {
                var calendarEvent = Events.Get(eventId);

                Apply(new Calendar_OnetimeCalendarEventChangeStatusEvent
                {
                    CalendarId = _id,
                    EventId = eventId,
                    NewStatus = newStatus,
                    // additional data
                    UserId = calendarEvent.UserId,
                    CreatedDate = calendarEvent.CreatedDate,
                    EventDate = calendarEvent.EventDate,
                    Description = calendarEvent.Description,
                    SendAlertOptions = calendarEvent.SendAlertOptions,
                    ParentId = calendarEvent.ParentId,
                });
            }
        }

        public void MarkRepeatingCalendarEventAsDone(string eventId)
        {
            Apply(new Calendar_RepeatingCalendarEventDoneEvent
            {
                CalendarId = _id,
                EventId = eventId
            });
        }

        #region Object Reconstruction

        protected void On(Calendar_CreatedEvent created)
        {
            _id = created.CalendarId;
            Events = new CalendarEventCollection();
        }

        protected void On(Calendar_OnetimeCalendarEventAddedEvent created)
        {
            var newEvent = new CalendarEvent
            {
                Id = created.CalendarEventId,
                CreatedDate = created.CreatedDate,
                EventDate = created.EventDate,
                Description = created.Description,
                IsDone = created.IsDone,
                SendAlertOptions = created.SendAlertOptions,
                ParentId = created.ParentId,
                UserId = created.UserId ?? created.Metadata.UserId,
            };
            Events.Add(newEvent);
        }

        protected void On(Calendar_OnetimeCalendarEventChangeStatusEvent changed)
        {
            Events.Get(changed.EventId).IsDone = changed.NewStatus;
        }

        protected void On(Calendar_OnetimeCalendarEventDeletedEvent changed)
        {
            Events.Remove(changed.EventId);
        }

        #endregion
    }
}
