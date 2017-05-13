using System.Collections.Generic;

namespace Default.ViewModel.Areas.Finance.GoalsController
{
    public class GoalSideBarModel
    {
        public List<GoalSideBarItemModel> ActiveItems { get; set; }

        public List<GoalSideBarItemModel> CompletedItems { get; set; } 
    }
}