using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.Documents.Calendar;

namespace mPower.Tests.UnitTests.Domain.Accounting.Calendar
{
    public class calendar_onetime_event_change_status_test : CalendarTest
    {
        private readonly string _calendarAdditionalEventId = Guid.NewGuid().ToString();

        public override IEnumerable<IEvent> Given()
        {
            yield return User_Created();
            yield return Calendar_Created();
            yield return Calendar_OnetimeEvent_Added();
            yield return Calendar_OnetimeEvent_Added(_calendarAdditionalEventId, false);
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Calendar_OnetimeEvent_ChangeStatus(_calendarEventId, true);
            yield return Calendar_OnetimeEvent_ChangeStatus(_calendarAdditionalEventId, false);
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Calendar_OnetimeEvent_StatusChanged(_calendarEventId, true);
            yield return Calendar_OnetimeEvent_StatusChanged(_calendarAdditionalEventId, false);
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                CalendarDocument doc = _calendarDocumentService.GetById(_id);
                CalendarEventDocument calendarEvent1 = doc.CalendarEvents.Single(e => e.Id == _calendarEventId);
                CalendarEventDocument calendarEvent2 = doc.CalendarEvents.Single(e => e.Id == _calendarAdditionalEventId);
                Assert.NotNull(calendarEvent1);
                Assert.IsTrue(calendarEvent1.IsDone);
                Assert.NotNull(calendarEvent2);
                Assert.IsTrue(!calendarEvent2.IsDone);
            });
        }
    }
}