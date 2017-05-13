namespace Default.ViewModel.Areas.Finance.BudgetController
{
    public class BudgetModel
    {
        public BudgetModel()
        {
            GraphModel = new BudgetGraphModel();
            SummaryModel = new BudgetSummaryModel();
            WizardModel = new BudgetWizardModel();
        }

        public BudgetGraphModel GraphModel { get; set; }

        public BudgetSummaryModel SummaryModel { get; set; }

        public BudgetWizardModel WizardModel { get; set; }

        public bool IsBudgetForLedgerSet { get; set; }

        public bool ShowBudgetWizard { get; set; }
    }
}
