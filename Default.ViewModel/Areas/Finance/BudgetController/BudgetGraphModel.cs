using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Finance.BudgetController
{
    public class BudgetGraphItemModel
    {
        public BudgetGraphItemModel()
        {
            SubBudgets = new List<BudgetGraphItemModel>();
        }

        public string Id { get; set; }

        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public long AmountBudgeted { get; set; }

        public long AmountSpent { get; set; }

        public int Persent { get; set; }

        public AccountTypeEnum AccountType { get; set; }

        public DateTime Date { get; set; }

        public string Color
        {
            get
            {
                string result = "";
                switch (AccountType)
                {
                    case AccountTypeEnum.Expense:
                        result = "green";
                        if (Persent > 50 && Persent < 100)
                            result = "yellow";
                        else if (Persent >= 100)
                            result = "red";
                        break;
                    case AccountTypeEnum.Income:
                        result = "red";
                        if (Persent > 33 && Persent <= 66)
                            result = "yellow";
                        else if (Persent > 66)
                            result = "green";
                        break;
                }


                return result;
            }
        }

        public List<BudgetGraphItemModel> SubBudgets { get; set; }
    }

    public class BudgetMonthModel
    {
        public string Text { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class BudgetGraphModel
    {


        public BudgetGraphModel()
        {
            Monthes = new List<BudgetMonthModel>();
            IncomeItems = new List<BudgetGraphItemModel>();
            ExpenseItems = new List<BudgetGraphItemModel>();
        }

        public DateTime Date { get; set; }

        public List<BudgetMonthModel> Monthes { get; set; }

        public long IncomeBudgetedTotal { get; set; }

        public long IncomeSpentTotal { get; set; }

        public List<BudgetGraphItemModel> IncomeItems { get; set; }

        public long ExpenseBudgetedTotal { get; set; }

        public long ExpenseSpentTotal { get; set; }

        public List<BudgetGraphItemModel> ExpenseItems { get; set; }
    }

    public class BudgetSummaryModel
    {
        public long BudgetedIncomeInCents { get; set; }

        public long BudgetedExpenseInCents { get; set; }

        public long ActualIncomeInCents { get; set; }

        public long ActualExpenseInCents { get; set; }
    }
}
