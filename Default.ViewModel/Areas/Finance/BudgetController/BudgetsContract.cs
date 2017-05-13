using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Default.ViewModel.Areas.Finance.BudgetController
{
    public class BudgetsContract
    {
        public List<BudgetItemContract> income { get; set; }

        public List<BudgetItemContract> expense { get; set; }
    }
}
