using System;
using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.DocumentServices.Goal;
using mPower.Documents.Enums;

namespace mPower.Accounting.Tests.UnitTests.Goals
{
    public class goal_creation_test : GoalTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield break;
        }

        public override IEnumerable<ICommand> When()
        {
            yield return CreateCommand();
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return CreatedEvent();
        }

        [Test]
        public void Test()
        {
            Validate();
            DispatchEvents(() =>
                               {
                                   var goalDocumentService = GetInstance<GoalDocumentService>();
                                   var goal = goalDocumentService.GetById(_id);
                                   Assert.AreEqual(goal.Title,_title);
                                   Assert.AreEqual(goal.PlannedDate.Date,_plannedDate.Date);
                                   Assert.AreEqual(goal.Image,_image);
                                   Assert.AreEqual(goal.MonthlyPlanAmountInCents,_monthlyPlanAmountInCents);
                                   Assert.AreEqual(goal.StartDate.Date,_startDate.Date);
                                   Assert.AreEqual(goal.TotalAmountInCents,_totalAmountInCents);
                                   Assert.AreEqual(goal.Type,_type);
                                   Assert.AreEqual(goal.Status,GoalStatusEnum.Projected);
                                   Assert.AreEqual(goal.CalcDate,null);
                               });
        }
    }
}