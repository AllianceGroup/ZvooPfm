using System;
using mPower.Documents.DocumentServices.Calendar;
using mPower.Domain.Accounting.Calendar;
using mPower.Domain.Accounting.Calendar.Commands;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.Calendar.Events;
using mPower.Domain.Accounting.Enums;
using Paralect.Domain;
using mPower.Domain.Membership.User.Events;
using mPower.Tests.Environment;

namespace mPower.Tests.UnitTests.Domain.Accounting.Calendar
{
    public abstract class CalendarTest : AggregateTest<CalendarAR>
    {
        protected CalendarTest()
        {
            _currentDate = DateTime.Now;
        }

        #region User Details

        protected string _userId = Guid.NewGuid().ToString();
        protected string _affiliateId = Guid.NewGuid().ToString();
        protected string _email = "an.orsich@gmail.com";
        protected string _firstName = "Andrew";
        protected string _lastName = "Orsich";
        protected string _password = "asd123";
        protected string _userName = "anorsich";
        protected DateTime _currentDate;

        #endregion

        protected string _calendarName = "Test Calendar";
        protected CalendarTypeEnum _calendarType = CalendarTypeEnum.Default;
        protected string _calendarEventId = Guid.NewGuid().ToString();
        protected string _calendarEventDescription = "Some Description";

        protected CalendarEventFrequencyEnum _repeatingEventFrequency = CalendarEventFrequencyEnum.Daily;
        protected int _repeatingEventPeriod = 100;
        protected DayOfWeek[] _repeatingEventDays = new[] {DayOfWeek.Monday, DayOfWeek.Sunday};
        protected RepeatingEventEndOption _repeatingEventEnd = new RepeatingEventEndOption { After = 10 };

        public IEvent User_Created()
        {
            return new User_CreatedEvent
            {
                UserId = _userId,
                ApplicationId = _affiliateId,
                Email = _email,
                FirstName = _firstName,
                LastName = _lastName,
                Password = _password,
                UserName = _userName,
                CreateDate = _currentDate,
                IsActive = true,
            };
        }

        public ICommand Calendar_Create()
        {
            return new Calendar_CreateCommand
            {
                CalendarId = _id,
                Name = _calendarName,
                Type = _calendarType,
            };
        }

        public IEvent Calendar_Created()
        {
            return new Calendar_CreatedEvent
            {
                CalendarId = _id,
                Name = _calendarName,
                Type = _calendarType,
            };
        }

        public ICommand Calendar_OnetimeEvent_Create()
        {
            return new Calendar_OnetimeCalendarEvent_CreateCommand
            {
                CalendarId = _id,
                UserId = _userId,
                CreatedDate = _currentDate,
                EventDate = _currentDate,
                Description = _calendarEventDescription,
                IsDone = false,
                CalendarEventId = _calendarEventId,
            };
        }

        public IEvent Calendar_OnetimeEvent_Added()
        {
            return Calendar_OnetimeEvent_Added(_calendarEventId, false);
        }

        public IEvent Calendar_OnetimeEvent_Added(string eventId, bool isDone)
        {
            return new Calendar_OnetimeCalendarEventAddedEvent
            {
                CalendarId = _id,
                UserId = _userId,
                CreatedDate = _currentDate,
                EventDate = _currentDate,
                Description = _calendarEventDescription,
                IsDone = isDone,
                CalendarEventId = eventId,
            };
        }

        public ICommand Calendar_OnetimeEvent_ChangeStatus(string eventId, bool newStatus)
        {
            return new Calendar_OnetimeCalendarEvent_ChangeStatusCommand
            {
                CalendarId = _id,
                EventId = eventId,
                IsDone = newStatus,
            };
        }

        public IEvent Calendar_OnetimeEvent_StatusChanged(string eventId, bool newStatus)
        {
            return new Calendar_OnetimeCalendarEventChangeStatusEvent
            {
                CalendarId = _id,
                EventId = eventId,
                NewStatus = newStatus,
                // additional data
                UserId = _userId,
                CreatedDate = _currentDate,
                EventDate = _currentDate,
                Description = _calendarEventDescription,
            };
        }

        public IEvent Calendar_RepeatingEvent_Added()
        {
            return new Calendar_RepeatingCalendarEventAddedEvent
            {
                UserId = _userId,
                CalendarId = _id,
                Date = _currentDate,
                Description = _calendarEventDescription,
                EventId = _calendarEventId,
                Frequency = _repeatingEventFrequency,
                Period = _repeatingEventPeriod,
                RepeatOn = _repeatingEventDays,
                StartDate = _currentDate,
                End = _repeatingEventEnd,
            };
        }

        protected CalendarDocumentService _calendarDocumentService
        {
            get
            {
                return GetInstance<CalendarDocumentService>();
            }
        }
    }
}