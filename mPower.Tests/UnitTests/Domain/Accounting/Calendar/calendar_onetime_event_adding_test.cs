using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.Documents.Calendar;

namespace mPower.Tests.UnitTests.Domain.Accounting.Calendar
{
    public class calendar_onetime_event_adding_test : CalendarTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield return User_Created();
            yield return Calendar_Created();
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Calendar_OnetimeEvent_Create();
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Calendar_OnetimeEvent_Added();
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                CalendarDocument doc = _calendarDocumentService.GetById(_id);
                CalendarEventDocument calendarEvent = doc.CalendarEvents.Single(e => e.Id == _calendarEventId);

                Assert.NotNull(calendarEvent);
                Assert.AreEqual(calendarEvent.CalendarId, _id);
                Assert.IsTrue((calendarEvent.Date - _currentDate).CompareTo(new TimeSpan(0, 0, 0, 0, 10)) < 0);
                Assert.AreEqual(calendarEvent.Description, _calendarEventDescription);
                Assert.AreEqual(calendarEvent.IsDone, false);
            });
        } 
    }
}