using System.Collections.Generic;

namespace Default.ViewModel.Areas.Finance.GoalsController
{
    public class GoalsListingModel
    {
        public List<GoalsListItemModel> ActiveItems { get; set; }

        public List<GoalsListItemModel> CompletedItems { get; set; }
        public List<GoalsListItemModel> ArchivedItems { get; set; }

        public long? AvailableAmountInDollars { get; set; }
        public int ActiveGoalsNumber { get; set; }
    }
}