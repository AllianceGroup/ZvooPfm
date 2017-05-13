using System;
using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.Areas.Finance.BudgetController;
using Default.ViewModel.Areas.Finance.BudgetController.Filters;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc;

namespace Default.Factories.ViewModels
{
    public class BudgetViewModelFactory :
        IObjectFactory<BudgetFilter, BudgetModel>,
        IObjectFactory<BudgetGraphFilter, BudgetGraphModel>,
        IObjectFactory<BudgetSummaryFilter, BudgetSummaryModel>,
        IObjectFactory<BudgetWizardFilter, BudgetWizardModel>
    {
        private readonly BudgetDocumentService _budgetService;

        public BudgetViewModelFactory(BudgetDocumentService budgetService)
        {
            _budgetService = budgetService;
        }

        public BudgetModel Load(BudgetFilter filter)
        {
            var model = new BudgetModel {IsBudgetForLedgerSet = filter.Ledger.IsBudgetSet};
            if (model.IsBudgetForLedgerSet)
            {
                model.GraphModel = Load(new BudgetGraphFilter {LedgerId = filter.Ledger.Id, Year = filter.Year, Month = filter.Month});
                model.SummaryModel = Load(new BudgetSummaryFilter {LedgerId = filter.Ledger.Id, Year = filter.Year, Month = filter.Month});
            }
            else // empty model in case if budget not set yet
            {
                model.GraphModel = new BudgetGraphModel {Monthes = BuildMonthesModel(), Date = DateTime.Now};
                model.SummaryModel = new BudgetSummaryModel();
            }

            model.ShowBudgetWizard = !string.IsNullOrEmpty(filter.Setup);
            model.WizardModel = Load(new BudgetWizardFilter {Ledger = filter.Ledger, Year = filter.Year, Month = filter.Month});

            return model;
        }

        public BudgetGraphModel Load(BudgetGraphFilter filter)
        {
            var model = new BudgetGraphModel {Date = new DateTime(filter.Year, filter.Month, 1, 23, 59, 59)};

            var expenseBudgets = new List<BudgetDocument>();
            var incomeBudgets = new List<BudgetDocument>();
            GetSortedBudgets(filter.Month, filter.Year, filter.LedgerId, expenseBudgets, incomeBudgets);

            model.ExpenseItems = expenseBudgets.Select(b => CreateBudgetGraphItem(b, model.Date)).ToList();
            model.IncomeItems = incomeBudgets.Select(b => CreateBudgetGraphItem(b, model.Date)).ToList();

            model.ExpenseBudgetedTotal = model.ExpenseItems.Sum(x => x.AmountBudgeted);
            model.ExpenseSpentTotal = model.ExpenseItems.Sum(x => x.AmountSpent);
            model.IncomeBudgetedTotal = model.IncomeItems.Sum(x => x.AmountBudgeted);
            model.IncomeSpentTotal = model.IncomeItems.Sum(x => x.AmountSpent);
            model.Monthes = BuildMonthesModel();

            return model;
        }

        public BudgetSummaryModel Load(BudgetSummaryFilter filter)
        {
            var expenseBudgets = new List<BudgetDocument>();
            var incomeBudgets = new List<BudgetDocument>();
            GetSortedBudgets(filter.Month, filter.Year, filter.LedgerId, expenseBudgets, incomeBudgets);

            var model = new BudgetSummaryModel
            {
                ActualExpenseInCents = expenseBudgets.Sum(x => x.SpentAmountWithSubAccounts),
                ActualIncomeInCents = incomeBudgets.Sum(x => x.SpentAmountWithSubAccounts),
                BudgetedExpenseInCents = expenseBudgets.Sum(x => x.BudgetAmount),
                BudgetedIncomeInCents = incomeBudgets.Sum(x => x.BudgetAmount)
            };

            return model;
        }

        public BudgetWizardModel Load(BudgetWizardFilter filter)
        {
            var model = new BudgetWizardModel {IsBudgetSetForLedger = filter.Ledger.IsBudgetSet};
            
            // sort budgets by type
            var expenseBudgets = new List<BudgetDocument>();
            var incomeBudgets = new List<BudgetDocument>();
            GetSortedBudgets(filter.Month, filter.Year, filter.Ledger.Id, expenseBudgets, incomeBudgets);

            // get details on income budgets
            var incomeAccounts = filter.Ledger.Accounts.Where(x => x.TypeEnum == AccountTypeEnum.Income).ToList();
            var incomeItems = GetBudgetItems(incomeAccounts, incomeBudgets, model.IsBudgetSetForLedger);
            model.IncomeItems = incomeItems.OrderBy(x => x.AccountName).ToList();
            model.TotalIncome = incomeItems.Sum(x => x.BudgetAmountInDollars);

            // get details on expense budgets
            var expenseAccounts = filter.Ledger.Accounts.Where(x => x.TypeEnum == AccountTypeEnum.Expense).ToList();
            var expenseItems = GetBudgetItems(expenseAccounts, expenseBudgets, model.IsBudgetSetForLedger);
            model.ExpenseItems = expenseItems.OrderBy(x => x.AccountName).ToList();
            model.TotalExpense = expenseItems.Sum(x => x.BudgetAmountInDollars);

            // date
            model.Month = filter.Month;
            model.Year = filter.Year;

            return model;
        }

        #region Helpers

        private static List<BudgetMonthModel> BuildMonthesModel()
        {
            var monthes = new List<BudgetMonthModel>();
            //showing previous 8 months
            var initialDate = DateTime.Now.AddMonths(-8);

            for (var i = 0; i < 12; i++)
            {
                var text = initialDate.Month == 1 
                    ? initialDate.ToString(@"MMM <\span cla\s\s='year'>yyyy</\span>")
                    : initialDate.ToString("MMM");

                monthes.Add(new BudgetMonthModel {Month = initialDate.Month, Year = initialDate.Year, Text = text});
                initialDate = initialDate.AddMonths(1);
            }

            return monthes;
        }

        private void GetSortedBudgets(int month, int year, string ledgerId, ICollection<BudgetDocument> expenseBudgets, ICollection<BudgetDocument> incomeBudgets)
        {
            var budgets = _budgetService.GetLedgetBudgetsByMonthAndYear(month, year, ledgerId);
            foreach (var budget in budgets)
            {
                switch (budget.AccountType)
                {
                    case AccountTypeEnum.Expense:
                        expenseBudgets.Add(budget);
                        break;
                    case AccountTypeEnum.Income:
                        incomeBudgets.Add(budget);
                        break;
                }
            }
        }

        private static BudgetGraphItemModel CreateBudgetGraphItem(BudgetDocument budget, DateTime date)
        {
            var parent = new BudgetGraphItemModel
            {
                AccountName = budget.AccountName,
                AmountBudgeted = budget.BudgetAmount,
                AmountSpent = budget.SpentAmountWithSubAccounts,
                Persent = budget.Persent,
                AccountId = budget.AccountId,
                Id = budget.Id,
                AccountType = budget.AccountType,
                Date = date,
            };

            foreach (var subBudget in budget.SubBudgets)
            {
                parent.SubBudgets.Add(new BudgetGraphItemModel
                {
                    AccountId = subBudget.AccountId,
                    AccountName = subBudget.AccountName,
                    AmountSpent = subBudget.SpentAmount,
                    Id = subBudget.AccountId
                });
            }

            return parent;
        }

        private static List<BudgetItemModel> GetBudgetItems(IEnumerable<AccountDocument> accounts, List<BudgetDocument> budgets, bool isBudgetSetForLedger)
        {
            var items = new List<BudgetItemModel>();
            foreach (var item in accounts.Where(x => String.IsNullOrEmpty(x.ParentAccountId)))
            {
                var budget = budgets.Find(x => x.AccountId == item.Id);
                items.Add(CreateBudgetItem(item, budget, isBudgetSetForLedger));
            }
            return items;
        }

        private static BudgetItemModel CreateBudgetItem(AccountDocument account, BudgetDocument budget, bool isBudgetSetForLedger)
        {
            return new BudgetItemModel
            {
                AccountId = account.Id,
                AccountName = account.Name,
                BudgetAmountInDollars = budget == null ? 0 : AccountingFormatter.CentsToDollars(budget.BudgetAmount),
                IsIncludedInBudget = budget != null || !isBudgetSetForLedger,
            };
        }

        #endregion
    }
}