using System.Collections.Generic;
using System.Linq;
using Default.Factories.Commands;
using Default.ViewModel.Areas.Finance.BudgetController;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Framework.Environment;
using mPower.Tests.MoqObjects.DocumentServices;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Web.Factories.Commands
{
    [TestFixture]
    public class Budget_SetCommandFactoryTest : BaseWebTest
    {
        private LedgerDocumentServiceMock _ledgerService;
        private BusinessReportDocumentServiceMock _businessReportService;
        private Budget_SetCommandFactory _factory;
        private BudgetDocumentServiceMock _budgetService;

        [SetUp]
        public void TestSetup()
        {
            _budgetService = MockFactory.Create<BudgetDocumentServiceMock>();
            _ledgerService = MockFactory.Create<LedgerDocumentServiceMock>().AddGetById();
            _businessReportService = MockFactory.Create<BusinessReportDocumentServiceMock>().AddGetProfitLossReportData();
            _factory = new Budget_SetCommandFactory(_ledgerService.Object, _container.GetInstance<IIdGenerator>(), _businessReportService.Object);
        }

        [Test]
        public void factory_correctly_loads_budget_set_command()
        {
            string ledgerId = _ledgerService.FakeLedger.Id;
            const int year = 2012, month = 1;
            var budgets = _budgetService.BudgetsWithSubBudgets.Where(x => x.LedgerId == ledgerId && x.Year == year && x.Month == month).ToList();
            var dto = new BudgetsListDto
            {
                LedgerId = ledgerId,
                Budgets = budgets.Select(Map).ToList(),
                Year = year,
                Month = month,
            };

            var command = _factory.Load(dto);

            Assert.AreEqual(dto.LedgerId, command.LedgerId);
            AssertAreEqual(budgets, command.Budgets);
        }

        #region Helpers

        private static BudgetItemContract Map(BudgetDocument doc)
        {
            return new BudgetItemContract
            {
                Id = doc.AccountId,
                Budget = AccountingFormatter.CentsToDollarString(doc.BudgetAmount),
                IncludeBudget = true,
            };
        }

        private static void AssertAreEqual(List<BudgetDocument> expected, List<BudgetData> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            var createdBudgetsIds = new List<string>();
            foreach (var expectedBudget in expected)
            {
                var actualBudget = actual.Find(x => x.AccountId == expectedBudget.AccountId); // don't search by Id because it is generated
                Assert.IsNotNull(actualBudget);
                Assert.IsFalse(string.IsNullOrEmpty(actualBudget.Id));
                Assert.IsFalse(createdBudgetsIds.Contains(actualBudget.Id));
                Assert.AreEqual(expectedBudget.AccountName, actualBudget.AccountName);
                Assert.AreEqual(expectedBudget.AccountType, actualBudget.AccountType);
                Assert.AreEqual(expectedBudget.Year, actualBudget.Year);
                Assert.AreEqual(expectedBudget.Month, actualBudget.Month);
                Assert.AreEqual(expectedBudget.ParentId, actualBudget.ParentId);
                Assert.AreEqual(expectedBudget.BudgetAmount, actualBudget.BudgetAmount);
                Assert.AreEqual(expectedBudget.SpentAmount, actualBudget.SpentAmount);
                AssertAreEqual(expectedBudget.SubBudgets, actualBudget.SubBudgets);
            }
        }

        private static void AssertAreEqual(List<ChildBudgetDocument> expected, List<BudgetData> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            var createdBudgetsIds = new List<string>();
            foreach (var expectedBudget in expected)
            {
                var actualBudget = actual.Find(x => x.AccountId == expectedBudget.AccountId); // don't search by Id because it is generated
                Assert.IsNotNull(actualBudget);
                Assert.IsFalse(string.IsNullOrEmpty(actualBudget.Id));
                Assert.IsFalse(createdBudgetsIds.Contains(actualBudget.Id));
                Assert.AreEqual(expectedBudget.AccountName, actualBudget.AccountName);
                Assert.AreEqual(expectedBudget.AccountType, actualBudget.AccountType);
                Assert.AreEqual(expectedBudget.ParentAccountId, actualBudget.ParentId);
                Assert.AreEqual(expectedBudget.SpentAmount, actualBudget.SpentAmount);
            }
        }

        #endregion
    }
}