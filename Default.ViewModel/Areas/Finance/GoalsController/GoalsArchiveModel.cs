using System.Collections.Generic;

namespace Default.ViewModel.Areas.Finance.GoalsController
{
    public class GoalsArchiveModel
    {
        public List<GoalsListItemModel> ArchivedItems { get; set; }

        public long? AvailableAmountInDollars { get; set; } 

        public long ActiveGoalsNumber { get; set; } 
    }
}