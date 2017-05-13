using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Finance.DebtElimninationProgramController
{
    public class SelectPlanModel
    {
        public DebtEliminationPlanEnum Plan { get; set; }
        public decimal MonthlyBudget { get; set; }

    }
}