using System;
using mPower.Documents.Enums;
using mPower.Framework.Utils.Extensions;

namespace Default.ViewModel.Areas.Finance.GoalsController
{
    public class GoalsListItemModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public GoalStatusEnum Status { get; set; }

        public DateTime PlannedDate { get; set; }

        public long TotalAmountInDollars { get; set; }

        public long StartingBalanceInDollars { get; set; }

        public long CurrentAmountInDollars { get; set; }

        public long MonthlyPlanAmountInDollars { get; set; }

        public long MonthlyActualAmountInDollars { get; set; }

        public int MonthsAheadNumber { get; set; }

        public string ImageName { get; set; }
        public string StatusName => Status.GetDescription();
    }
}