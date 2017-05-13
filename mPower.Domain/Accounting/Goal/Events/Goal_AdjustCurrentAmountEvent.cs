using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Goal.Events
{
    public class Goal_AdjustCurrentAmountEvent : Event
    {
        public long ValueInCents { get; set; }

        public string GoalId { get; set; }

        public DateTime Date { get; set; }
    }
}