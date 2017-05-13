using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Goal.Commands
{
    public class Goal_AdjustCurrentAmountCommand : Command
    {
        public string GoalId { get; set; }

        public long ValueInCents { get; set; }

        public DateTime Date { get; set; }
    }
}