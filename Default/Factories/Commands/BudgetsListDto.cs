using System.Collections.Generic;
using Default.ViewModel.Areas.Finance.BudgetController;

namespace Default.Factories.Commands
{
    public class BudgetsListDto
    {
        public string LedgerId { get; set; }

        public List<BudgetItemContract> Budgets { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }
    }
}