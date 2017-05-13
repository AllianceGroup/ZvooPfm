using mPower.Documents.Documents.Accounting.Ledger;

namespace Default.ViewModel.Areas.Finance.BudgetController.Filters
{
    public class BudgetWizardFilter
    {
        public LedgerDocument Ledger { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }
    }
}