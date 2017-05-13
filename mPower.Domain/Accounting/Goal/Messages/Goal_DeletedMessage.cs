using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Goal.Messages
{
    public class Goal_DeletedMessage : Event
    {
        public string GoalId { get; set; }

        public GoalTypeEnum Type { get; set; }

        public string UserId { get; set; }

        public DateTime StartDate { get; set; }
    }
}