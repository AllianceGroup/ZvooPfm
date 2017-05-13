using System;
using System.Collections.Generic;
using System.Linq;
using Default.Factories.ViewModels;
using Default.ViewModel.Areas.Finance.BudgetController;
using Default.ViewModel.Areas.Finance.BudgetController.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Tests.MoqObjects.DocumentServices;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Web.Factories.ViewModel
{
    [TestFixture]
    public class BudgetViewModelFactoryTest : BaseWebTest
    {
        private BudgetViewModelFactory _factory;
        private BudgetDocumentServiceMock _budgetService;
        private LedgerDocumentServiceMock _ledgerService;

        private LedgerDocument _fakeLedger;
        private LedgerDocument FakeLedger
        {
            get
            {
                if (_fakeLedger == null)
                {
                    _fakeLedger = _ledgerService.FakeLedger;
                    _fakeLedger.IsBudgetSet = true;
                }

                return _fakeLedger;
            }
        }

        [SetUp]
        public void TestSetup()
        {
            _budgetService = MockFactory.Create<BudgetDocumentServiceMock>().AddGetLedgetBudgetsByMonthAndYear();
            _ledgerService = MockFactory.Create<LedgerDocumentServiceMock>();
            _factory = new BudgetViewModelFactory(_budgetService.Object);
        }

        [Test]
        public void factory_correctly_loads_budget_wizard_model()
        {
            var filter = new BudgetWizardFilter
            {
                Ledger = FakeLedger,
                Year = 2012,
                Month = 1,
            };
            var model = _factory.Load(filter);

            AssertIsCorrect(model, filter);
        }

        [Test]
        public void factory_correctly_loads_budget_summary_model()
        {
            var filter = new BudgetSummaryFilter
            {
                LedgerId = FakeLedger.Id,
                Year = 2012,
                Month = 1,
            };
            var model = _factory.Load(filter);

            AssertIsCorrect(model, filter);
        }

        [Test]
        public void factory_correctly_loads_budget_graph_model()
        {
            var filter = new BudgetGraphFilter
            {
                LedgerId = FakeLedger.Id,
                Year = 2012,
                Month = 1,
            };
            var model = _factory.Load(filter);

            AssertIsCorrect(model, filter);
        }

        [Test]
        public void factory_correctly_loads_budget_model()
        {
            var filter = new BudgetFilter
            {
                Ledger = FakeLedger,
                Year = 2012,
                Month = 1,
                Setup = "foo",
            };
            var model = _factory.Load(filter);

            Assert.AreEqual(FakeLedger.IsBudgetSet, model.IsBudgetForLedgerSet);
            Assert.AreEqual(!string.IsNullOrEmpty(filter.Setup), model.ShowBudgetWizard);
            AssertIsCorrect(model.WizardModel, new BudgetWizardFilter { Ledger = filter.Ledger, Year = filter.Year, Month = filter.Month });
            AssertIsCorrect(model.SummaryModel, new BudgetSummaryFilter { LedgerId = filter.Ledger.Id, Year = filter.Year, Month = filter.Month });
            AssertIsCorrect(model.GraphModel, new BudgetGraphFilter { LedgerId = filter.Ledger.Id, Year = filter.Year, Month = filter.Month });
        }

        [Test]
        public void factory_correctly_loads_empty_budget_model()
        {
            FakeLedger.IsBudgetSet = false;
            var filter = new BudgetFilter
            {
                Ledger = FakeLedger,
                Year = 2012,
                Month = 1,
                Setup = "foo",
            };
            var model = _factory.Load(filter);

            Assert.AreEqual(FakeLedger.IsBudgetSet, model.IsBudgetForLedgerSet);
            Assert.AreEqual(!string.IsNullOrEmpty(filter.Setup), model.ShowBudgetWizard);
            AssertIsCorrect(model.WizardModel, new BudgetWizardFilter { Ledger = filter.Ledger, Year = filter.Year, Month = filter.Month });
            AssertIsEmpty(model.SummaryModel);
            AssertIsEmpty(model.GraphModel);
        }

        #region Helpers

        private void AssertIsCorrect(BudgetWizardModel model, BudgetWizardFilter filter)
        {
            // common data
            Assert.AreEqual(filter.Year, model.Year);
            Assert.AreEqual(filter.Month, model.Month);
            Assert.AreEqual(filter.Ledger.IsBudgetSet, model.IsBudgetSetForLedger);

            var budgets = _budgetService.Object.GetLedgetBudgetsByMonthAndYear(filter.Month, filter.Year, filter.Ledger.Id);
            // income budgets

            var incomeAccounts = FakeLedger.Accounts.Where(x => x.TypeEnum == AccountTypeEnum.Income && string.IsNullOrEmpty(x.ParentAccountId)).ToList();
            var incomeBudgets = budgets.Where(x => x.AccountType == AccountTypeEnum.Income).ToList();
            AssertAreEqual(incomeAccounts, incomeBudgets, model.IncomeItems);
            Assert.AreEqual(AccountingFormatter.CentsToDollars(incomeBudgets.Sum(x => x.BudgetAmount)), model.TotalIncome); // total

            // expense budgets
            var expenseAccounts = FakeLedger.Accounts.Where(x => x.TypeEnum == AccountTypeEnum.Expense && string.IsNullOrEmpty(x.ParentAccountId)).ToList();
            var expenseBudgets = budgets.Where(x => x.AccountType == AccountTypeEnum.Expense).ToList();
            AssertAreEqual(expenseAccounts, expenseBudgets, model.ExpenseItems);
            Assert.AreEqual(AccountingFormatter.CentsToDollars(expenseBudgets.Sum(x => x.BudgetAmount)), model.TotalExpense); // total
        }

        private void AssertIsCorrect(BudgetSummaryModel model, BudgetSummaryFilter filter)
        {
            var budgets = _budgetService.Object.GetLedgetBudgetsByMonthAndYear(filter.Month, filter.Year, filter.LedgerId);
            var incomeBudgets = budgets.Where(x => x.AccountType == AccountTypeEnum.Income).ToList();
            var expenseBudgets = budgets.Where(x => x.AccountType == AccountTypeEnum.Expense).ToList();

            var expectedBudgetedIncome = incomeBudgets.Sum(x => x.BudgetAmount);
            var expectedBudgetedExpense = expenseBudgets.Sum(x => x.BudgetAmount);
            var expectedActualIncome = incomeBudgets.Sum(x => x.SpentAmountWithSubAccounts);
            var expectedActualExpense = expenseBudgets.Sum(x => x.SpentAmountWithSubAccounts);

            Assert.AreEqual(expectedBudgetedIncome, model.BudgetedIncomeInCents);
            Assert.AreEqual(expectedBudgetedExpense, model.BudgetedExpenseInCents);
            Assert.AreEqual(expectedActualIncome, model.ActualIncomeInCents);
            Assert.AreEqual(expectedActualExpense, model.ActualExpenseInCents);
        }

        private void AssertIsCorrect(BudgetGraphModel model, BudgetGraphFilter filter)
        {
            var budgets = _budgetService.Object.GetLedgetBudgetsByMonthAndYear(filter.Month, filter.Year, filter.LedgerId);
            var incomeBudgets = budgets.Where(x => x.AccountType == AccountTypeEnum.Income).ToList();
            var expenseBudgets = budgets.Where(x => x.AccountType == AccountTypeEnum.Expense).ToList();

            AssertAreEqual(incomeBudgets, model.IncomeItems);
            AssertAreEqual(expenseBudgets, model.ExpenseItems);

            var expectedBudgetedIncome = incomeBudgets.Sum(x => x.BudgetAmount);
            var expectedBudgetedExpense = expenseBudgets.Sum(x => x.BudgetAmount);
            var expectedActualIncome = incomeBudgets.Sum(x => x.SpentAmountWithSubAccounts);
            var expectedActualExpense = expenseBudgets.Sum(x => x.SpentAmountWithSubAccounts);

            Assert.AreEqual(expectedBudgetedIncome, model.IncomeBudgetedTotal);
            Assert.AreEqual(expectedBudgetedExpense, model.ExpenseBudgetedTotal);
            Assert.AreEqual(expectedActualIncome, model.IncomeSpentTotal);
            Assert.AreEqual(expectedActualExpense, model.ExpenseSpentTotal);

            Assert.AreEqual(new DateTime(filter.Year, filter.Month, 1), model.Date);

            AssertIsCorrect(model.Monthes);
        }

        private static void AssertIsCorrect(List<BudgetMonthModel> model)
        {
            Assert.AreEqual(12, model.Count);
            var expectedDate = DateTime.Now.AddMonths(-8);
            foreach (var monthModel in model)
            {
                Assert.AreEqual(expectedDate.Year, monthModel.Year);
                Assert.AreEqual(expectedDate.Month, monthModel.Month);
                var expectedText = expectedDate.Month == 1
                    ? expectedDate.ToString(@"MMM <\span cla\s\s='year'>yyyy</\span>")
                    : expectedDate.ToString("MMM");
                Assert.AreEqual(expectedText, monthModel.Text);

                expectedDate = expectedDate.AddMonths(1);
            }
        }

        private static void AssertIsEmpty(BudgetSummaryModel model)
        {
            Assert.AreEqual(0, model.BudgetedIncomeInCents);
            Assert.AreEqual(0, model.BudgetedExpenseInCents);
            Assert.AreEqual(0, model.ActualIncomeInCents);
            Assert.AreEqual(0, model.ActualExpenseInCents);
        }

        private static void AssertIsEmpty(BudgetGraphModel model)
        {
            const int timeoutInSeconds = 10;
            Assert.LessOrEqual((DateTime.Now - model.Date).Seconds, timeoutInSeconds);
            AssertIsCorrect(model.Monthes);

            Assert.AreEqual(0, model.IncomeItems.Count);
            Assert.AreEqual(0, model.IncomeBudgetedTotal);
            Assert.AreEqual(0, model.IncomeSpentTotal);

            Assert.AreEqual(0, model.ExpenseItems.Count);
            Assert.AreEqual(0, model.ExpenseBudgetedTotal);
            Assert.AreEqual(0, model.ExpenseSpentTotal);
        }

        private void AssertAreEqual(List<AccountDocument> expectedAccounts, List<BudgetDocument> expectedBudgets, List<BudgetItemModel> actual)
        {
            Assert.AreEqual(expectedAccounts.Count, actual.Count);
            foreach (var accountDocument in expectedAccounts)
            {
                var budgetDocument = expectedBudgets.Find(x => x.AccountId == accountDocument.Id);
                var budgetItemModel = actual.Find(x => x.AccountId == accountDocument.Id);
                Assert.IsNotNull(budgetItemModel);
                AssertAreEqual(accountDocument, budgetDocument, budgetItemModel);
            }
        }

        private void AssertAreEqual(AccountDocument expectedAccount, BudgetDocument expectedBudget, BudgetItemModel actual)
        {
            Assert.AreEqual(expectedAccount.Id, actual.AccountId);
            Assert.AreEqual(expectedAccount.Name, actual.AccountName);
            Assert.AreEqual(AccountingFormatter.CentsToDollars(expectedBudget == null ? 0 : expectedBudget.BudgetAmount), actual.BudgetAmountInDollars);
            Assert.AreEqual(expectedBudget != null || !FakeLedger.IsBudgetSet, actual.IsIncludedInBudget);
        }

        private static void AssertAreEqual(List<BudgetDocument> expected, List<BudgetGraphItemModel> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            foreach (var expectedBudget in expected)
            {
                var actualBudget = actual.Find(x => x.Id == expectedBudget.Id);
                Assert.IsNotNull(actualBudget);
                AssertAreEqual(expectedBudget, actualBudget);
            }
        }

        private static void AssertAreEqual(BudgetDocument expected, BudgetGraphItemModel actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.AccountId, actual.AccountId);
            Assert.AreEqual(expected.AccountName, actual.AccountName);
            Assert.AreEqual(expected.AccountType, actual.AccountType);
            Assert.AreEqual(expected.BudgetAmount, actual.AmountBudgeted);
            Assert.AreEqual(expected.SpentAmountWithSubAccounts, actual.AmountSpent);
            Assert.AreEqual(expected.Persent, actual.Persent);
            Assert.AreEqual(new DateTime(expected.Year, expected.Month, 1), actual.Date);
        }

        #endregion
    }
}