using System.Collections.Generic;
using mPower.Domain.Accounting.Calendar.Commands;
using mPower.Domain.Accounting.Calendar.Events;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.Domain.Accounting.Calendar
{
    public class calendar_deletion_test : CalendarTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield return Calendar_Created();
        }

        public override IEnumerable<ICommand> When()
        {
            yield return new Calendar_DeleteCommand { CalendarId = _id };
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return new Calendar_DeletedEvent { CalendarId = _id };
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() => Assert.IsNull(_calendarDocumentService.GetById(_id)));
        }
    }
}