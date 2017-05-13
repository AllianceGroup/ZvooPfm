using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Calendar;
using mPower.Domain.Accounting.Calendar.Commands;
using mPower.Domain.Accounting.Calendar.Events;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.Domain.Accounting.Calendar
{
    public class calendar_repeating_event_deleting_test : CalendarTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield return Calendar_Created();
            yield return Calendar_RepeatingEvent_Added();
        }

        public override IEnumerable<ICommand> When()
        {
            yield return new Calendar_RepeatingCalendarEvent_DeleteCommand
            {
                CalendarId = _id,
                EventId = _calendarEventId,
            };
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return new Calendar_RepeatingCalendarEventDeletedEvent
            {
                CalendarId = _id,
                EventId = _calendarEventId
            };
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                CalendarDocument doc = _calendarDocumentService.GetById(_id);
                Assert.AreEqual(doc.CalendarEvents.Count(e => e.Id == _calendarEventId), 0);
            });
        }
    }
}