namespace Default.ViewModel.Areas.Finance.BudgetController
{
    public class BudgetItemContract
    {
        public string Id { get; set; }

        public string Budget { get; set; }

        public bool IncludeBudget { get; set; }
    }
}