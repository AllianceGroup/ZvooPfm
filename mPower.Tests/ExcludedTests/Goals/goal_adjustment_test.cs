using System;
using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.DocumentServices.Goal;
using mPower.Domain.Accounting.Goal.Commands;
using mPower.Domain.Accounting.Goal.Events;

namespace mPower.Accounting.Tests.UnitTests.Goals
{
    public class goal_adjustment_test : GoalTest
    {
        private long _adjustmentValue = 10000;

        public override IEnumerable<IEvent> Given()
        {
            yield return CreatedEvent();
        }

        public override IEnumerable<ICommand> When()
        {
            yield return new Goal_AdjustCurrentAmountCommand()
                             {
                                 GoalId = _id,
                                 ValueInCents = _adjustmentValue
                             };
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return new Goal_AdjustCurrentAmountEvent()
                             {
                                 GoalId = _id,
                                 ValueInCents = _adjustmentValue
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
                                   Assert.AreEqual(goal.CurrentAmountInCents,_adjustmentValue);
                                   Assert.AreEqual(goal.LatestMonthAdjustmentInCents,_adjustmentValue);
                               });
        }
    }
}