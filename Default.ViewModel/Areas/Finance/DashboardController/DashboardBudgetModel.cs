using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Finance.DashboardController
{
    public class DashboardBudgetModel
    {
        public string AccountName { get; set; }

        public int Percentage { get; set; }

        public long SpentAmount { get; set; }

        public long BudgetAmount { get; set; }

        public AccountTypeEnum AccountType { get; set; }

        public string Color
        {
            get
            {
                string result = "";
                switch (AccountType)
                {
                    case AccountTypeEnum.Expense:
                        result = "green";
                        if (Percentage > 50 && Percentage < 100)
                            result = "yellow";
                        else if (Percentage >= 100)
                            result = "red";
                        break;
                    case AccountTypeEnum.Income:
                        result = "red";
                        if (Percentage > 33 && Percentage <= 66)
                            result = "yellow";
                        else if (Percentage > 66)
                            result = "green";
                        break;
                }


                return result;
            }
        }
    }
}
