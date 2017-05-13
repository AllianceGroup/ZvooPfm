using System;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Goal.Data
{
    public class GoalData
    {
        public GoalTypeEnum Type { get; set; }

        public long TotalAmountInCents { get; set; }

        public long MonthlyPlanAmountInCents { get; set; }

        public long StartingBalanceInCents { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime PlannedDate { get; set; }

        public DateTime ProjectedDate { get; set; }

        public string UserId { get; set; }

        public string Image { get; set; } 
    }
}