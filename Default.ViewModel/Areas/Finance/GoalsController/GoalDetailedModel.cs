using System;
using mPower.Documents.Enums;

namespace Default.ViewModel.Areas.Finance.GoalsController
{
    public class GoalDetailedModel
    {
        public GoalSideBarModel SideBar { get; set; }

        public long? AvailableAmountInDollars { get; set; }

        #region Selected goal details

        public string Id { get; set; }

        public string Title { get; set; }

        public GoalStatusEnum Status { get; set; }

        public DateTime PlannedDate { get; set; }

        public DateTime ProjectedDate { get; set; }

        public long TotalAmountInDollars { get; set; }

        public long CurrentAmountInDollars { get; set; }

        public long MonthlyActualAmountInDollars { get; set; }

        public long StartingBalanceInDollars { get; set; }

        public int MonthsAheadNumber { get; set; }

        #endregion
    }
}