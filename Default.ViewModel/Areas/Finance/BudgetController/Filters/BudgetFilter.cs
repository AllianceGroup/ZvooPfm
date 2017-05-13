using mPower.Documents.Documents.Accounting.Ledger;

namespace Default.ViewModel.Areas.Finance.BudgetController.Filters
{
    public class BudgetFilter
    {
        public LedgerDocument Ledger { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public string Setup { get; set; }
    }
}