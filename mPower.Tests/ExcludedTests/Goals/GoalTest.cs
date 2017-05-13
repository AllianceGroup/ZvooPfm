using System;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Goal;
using mPower.Domain.Accounting.Goal.Commands;
using mPower.Domain.Accounting.Goal.Events;
using mPower.Tests.Environment;


namespace mPower.Accounting.Tests.UnitTests.Goals
{
    public abstract class GoalTest : AggregateTest<GoalAR>
    {
        protected GoalTypeEnum _type = GoalTypeEnum.BuyHome;
        protected string _title = "Demo home";
        protected long _totalAmountInCents = 10000000;
        protected long _monthlyPlanAmountInCents = (int)Math.Round(10000000 / 12.0, 0);
        protected DateTime _startDate = DateTime.Now;
        protected DateTime _plannedDate = DateTime.Now.AddYears(20);
        protected string _userId;
        protected string _image = String.Empty;

        public Goal_CreatedEvent CreatedEvent()
        {
            return new Goal_CreatedEvent()
            {
                GoalId = _id,
                Type = _type,
                Title = _title,
                TotalAmountInCents = _totalAmountInCents,
                MonthlyPlanAmountInCents = _monthlyPlanAmountInCents,
                StartDate = _startDate,
                PlannedDate = _plannedDate,
                UserId = _userId,
                Image = _image
            };
        }

        public Goal_CreateCommand CreateCommand()
        {
            return new Goal_CreateCommand()
            {
                GoalId = _id,
                Type = _type,
                Title = _title,
                TotalAmountInCents = _totalAmountInCents,
                MonthlyPlanAmountInCents = _monthlyPlanAmountInCents,
                StartDate = _startDate,
                PlannedDate = _plannedDate,
                UserId = _userId,
                Image = _image,
            };
        }
    }
}