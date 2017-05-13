using System;
using mPower.Documents.Enums;

namespace Default.ViewModel.Areas.Finance.GoalsController
{
    public class GoalSideBarItemModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public GoalStatusEnum Status { get; set; }

        public DateTime PlannedDate { get; set; }

        public string ImageName { get; set; }

        public bool IsActive { get; set; }
    }
}