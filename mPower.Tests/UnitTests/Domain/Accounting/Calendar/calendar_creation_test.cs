using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.Domain.Accounting.Calendar
{
    public class calendar_creation_test : CalendarTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield break;
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Calendar_Create();
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Calendar_Created();
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var calendar = _calendarDocumentService.GetById(_id);
                Assert.NotNull(calendar);
                Assert.AreEqual(calendar.Id, _id);
                Assert.AreEqual(calendar.LedgerId, null);
                Assert.AreEqual(calendar.Name, _calendarName);
                Assert.AreEqual(calendar.Type, _calendarType);
            });
        }
    }
}