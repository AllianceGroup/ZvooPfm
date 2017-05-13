using System.Collections.Generic;

namespace Default.ViewModel.GettingStartedController
{
    public class InterAccountDuplicatesViewModel
    {
        public string DuplicateId { get; set; }
        public InterAccountDuplicateEntry BaseTransaction { get; set; }
        public IEnumerable<InterAccountDuplicateEntry> PotentialDuplicates { get; set; }
        public int DuplicateCount { get; set; }

    }
}