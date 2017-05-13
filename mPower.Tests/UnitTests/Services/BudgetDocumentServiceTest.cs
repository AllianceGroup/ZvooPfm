using System;
using System.Collections.Generic;
using NUnit.Framework;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;

namespace mPower.Tests.UnitTests.Services
{
    public class BudgetDocumentServiceTest : BaseServiceTest
    {
        private BudgetDocumentService _entryDocumentService;

        public override void Setup()
        {
            base.Setup();
            _entryDocumentService = _container.GetInstance<BudgetDocumentService>();
        }

        public override IEnumerable<object> Given()
        {
            yield return new BudgetDocument
            {
                AccountId = "1_5",
                AccountName = "1_5",
                AccountType = AccountTypeEnum.Expense,
                BudgetAmount = 100,
                LedgerId = "1",
                Id = "1",
                Year = 2012,
                Month = 1,
                SpentAmount = 20,
                SubBudgets = new List<ChildBudgetDocument>
                {
                    new ChildBudgetDocument
                    {
                        AccountId = "1_51",
                        AccountName = "1_51",
                        AccountType = AccountTypeEnum.Expense,
                        SpentAmount = 20,
                        ParentAccountId = "1_5"
                    },
                    new ChildBudgetDocument
                    {
                        AccountId = "1_52",
                        AccountName = "1_52",
                        AccountType = AccountTypeEnum.Expense,
                        SpentAmount = 33,
                        ParentAccountId = "1_5"
                    },
                }
            };
            yield return new BudgetDocument
            {
                AccountId = "1_6",
                LedgerId = "1",
                Id = "2",
                AccountName = "1_6",
                AccountType = AccountTypeEnum.Income,
                Year = 2011,
                Month = 12,
                BudgetAmount = 150,
                SpentAmount = 33
            };
            yield return new BudgetDocument
            {
                AccountId = "1_6",
                LedgerId = "1",
                Id = "3",
                AccountName = "1_6",
                AccountType = AccountTypeEnum.Income,
                Year = 2012,
                Month = 1,
                BudgetAmount = 150,
                SpentAmount = 33
            };
            yield return new BudgetDocument
            {
                AccountId = "1_6",
                LedgerId = "1",
                Id = "4",
                AccountName = "1_6",
                AccountType = AccountTypeEnum.Income,
                Year = 2012,
                Month = 2,
                BudgetAmount = 150,
                SpentAmount = 33
            };
            yield return new BudgetDocument
            {
                AccountId = "2_0",
                LedgerId = "2",
                Id = "5",
                AccountName = "2_0",
                AccountType = AccountTypeEnum.Income,
                Year = 2011,
                Month = 11,
                BudgetAmount = 150,
                SpentAmount = 33
            };
            yield return new BudgetDocument
            {
                AccountId = "2_0",
                LedgerId = "2",
                Id = "6",
                AccountName = "2_0",
                AccountType = AccountTypeEnum.Income,
                Year = 2012,
                Month = 1,
                BudgetAmount = 150,
                SpentAmount = 33
            };
        }

        [Test]
        public void service_corretly_search_budgets_by_ledgerId()
        {
            var budgets = _entryDocumentService.GetByFilter(new BudgetFilter {LedgerId = "2"});

            Assert.AreEqual(2, budgets.Count);
        }

        [Test]
        public void service_corretly_search_budgets_by_accountId()
        {
            var budgets = _entryDocumentService.GetByFilter(new BudgetFilter {AccountId = "1_6"});

            Assert.AreEqual(3, budgets.Count);
        }

        [Test]
        public void service_corretly_search_budgets_by_year()
        {
            var budgets = _entryDocumentService.GetByFilter(new BudgetFilter {Year = 2011});

            Assert.AreEqual(2, budgets.Count);
        }

        [Test]
        public void service_corretly_search_budgets_by_month()
        {
            var budgets = _entryDocumentService.GetByFilter(new BudgetFilter {Month = 12});

            Assert.AreEqual(1, budgets.Count);
        }

        [Test]
        public void service_corretly_search_budgets_by_year_plus_month_from()
        {
            var budgets = _entryDocumentService.GetByFilter(new BudgetFilter {YearPlusMonthFrom = 2012*12 + 1});

            Assert.AreEqual(4, budgets.Count);
        }

        [Test]
        public void service_corretly_search_budgets_by_year_plus_month_to()
        {
            var budgets = _entryDocumentService.GetByFilter(new BudgetFilter {YearPlusMonthTo = 2011*12 + 12});

            Assert.AreEqual(2, budgets.Count);
        }

        [Test]
        public void service_corretly_search_budgets_by_year_plus_month_from_to()
        {
            var budgets = _entryDocumentService.GetByFilter(new BudgetFilter {YearPlusMonthFrom = 2011*12 + 12, YearPlusMonthTo = 2012*12 + 1});

            Assert.AreEqual(4, budgets.Count);
        }

        [Test]
        public void service_corretly_search_budgets_by_account_type()
        {
            var budgets = _entryDocumentService.GetByFilter(new BudgetFilter {AccountType = AccountTypeEnum.Expense});

            Assert.AreEqual(1, budgets.Count);
        }

        [Test]
        public void service_corretly_search_budgets_by_ledgerId_year_month()
        {
            var budgets = _entryDocumentService.GetLedgetBudgetsByMonthAndYear(1, 2012, "1");

            Assert.AreEqual(2, budgets.Count);
        }

        [Test]
        public void service_corretly_search_nearest_budgets_for_previous_month()
        {
            var budgets = _entryDocumentService.GetNearestBudgets(1, 2011, "1");

            Assert.AreEqual(1, budgets.Count);
            Assert.AreEqual("2", budgets[0].Id);
        }
        
        [Ignore]
        [Test]
        public void service_corretly_search_nearest_budgets_for_next_month()
        {
            var budgets = _entryDocumentService.GetNearestBudgets(1, 2013, "1");

            Assert.AreEqual(1, budgets.Count);
            Assert.AreEqual("4", budgets[0].Id);
        }
        
        [Test]
        public void service_corretly_search_for_half_year_to_date_budgets()
        {
            var budgetsTopBorderLimited = _entryDocumentService.GetHalfYearToDateBudgets("1", new DateTime(2012, 1, 20));
            Assert.AreEqual(3, budgetsTopBorderLimited.Count);

            var budgetsBottomBorderLimited = _entryDocumentService.GetHalfYearToDateBudgets("1", new DateTime(2012, 7, 5));
            Assert.AreEqual(1, budgetsBottomBorderLimited.Count);
        }
    }
}