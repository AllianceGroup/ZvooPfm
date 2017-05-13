using System;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Goal.Commands
{
    public class Goal_UpdateCommand : Command
    {
        public string GoalId { get; set; }

        public long TotalAmountInCents { get; set; }

        public long MonthlyPlanAmountInCents { get; set; }

        public string Title { get; set; }

        public DateTime PlannedDate { get; set; }

        public DateTime ProjectedDate { get; set; }

        public string Image { get; set; }

        public long StartingBalanceInCents { get; set; }
    }
}