using Default.ViewModel.Areas.Finance.DebtToIncomeRatioController;
using mPower.Documents.Documents.Accounting.DebtElimination;

namespace Default.ViewModel.Areas.Finance.DebtToolsController
{
    public class DebtToolsModel
    {
        public DebtEliminationShortModel DebtElimination { get; set; }

        public DebtToIncomeRatioModel DebtToIncomeRatio { get; set; }

        public MortgageAccelerationProgramDocument CurrentMortgageProgram { get; set; }
    }
}
