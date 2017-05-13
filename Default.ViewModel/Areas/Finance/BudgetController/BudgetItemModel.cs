namespace Default.ViewModel.Areas.Finance.BudgetController
{
    public class BudgetItemModel
    {
        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public decimal BudgetAmountInDollars { get; set; }

        public bool IsIncludedInBudget { get; set; }
    }
}