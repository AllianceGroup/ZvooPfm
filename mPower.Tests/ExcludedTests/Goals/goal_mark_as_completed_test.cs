using System;
using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.DocumentServices.Goal;
using mPower.Documents.Enums;
using mPower.Domain.Accounting.Goal.Commands;
using mPower.Domain.Accounting.Goal.Events;

namespace mPower.Accounting.Tests.UnitTests.Goals
{
    public class goal_mark_as_completed_test: GoalTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield return CreatedEvent();
        }

        public override IEnumerable<ICommand> When()
        {
            yield return new Goal_MarkAsCompletedCommand()
                             {
                                 GoalId = _id
                             };
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return new Goal_MarkedAsCompletedEvent()
                             {
                                 GoalId = _id
                             };
        }

        [Test]
        public void Test()
        {
            Validate();
            DispatchEvents(() =>
            {
                var goalDocumentService = GetInstance<GoalDocumentService>();
                var goal = goalDocumentService.GetById(_id);
                Assert.AreEqual(goal.Status, GoalStatusEnum.Completed);
            });
        }
    }
}