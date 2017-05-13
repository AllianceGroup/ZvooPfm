using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Default.ViewModel.Areas.Finance.BudgetController
{
    public class BudgetWizardModel
    {
        public bool IsBudgetSetForLedger { get; set; }

        public List<BudgetItemModel> IncomeItems { get; set; }

        public List<BudgetItemModel> ExpenseItems { get; set; }

        public decimal TotalIncome { get; set; }

        public decimal TotalExpense { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}
