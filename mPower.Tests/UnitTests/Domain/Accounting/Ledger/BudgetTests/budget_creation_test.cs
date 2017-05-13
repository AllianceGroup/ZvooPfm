using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Data;

namespace mPower.Tests.UnitTests.Domain.Accounting.Ledger.BudgetTests
{
    public class budget_creation_test : LedgerTest
    {
        private readonly List<BudgetData> _budgets;

        public budget_creation_test()
        {
            _budgets = new List<BudgetData>
            {
                new BudgetData
                {
                    Id = "5",
                    AccountId = "5",
                    AccountName = "5",
                    AccountType = AccountTypeEnum.Expense,
                    Month = CurrentDate.Month,
                    Year = CurrentDate.Year,
                    BudgetAmount = 100,
                    SpentAmount = 20,
                    SubBudgets = new List<BudgetData>
                    {
                        new BudgetData
                        {
                            Id = "51",
                            AccountId = "51",
                            AccountName = "51",
                            AccountType = AccountTypeEnum.Expense,
                            Month = CurrentDate.Month,
                            Year = CurrentDate.Year,
                            BudgetAmount = 0,
                            SpentAmount = 20,
                            ParentId = "5",
                        }
                    },
                },
                new BudgetData
                {
                    Id = "6",
                    AccountId = "6",
                    AccountName = "6",
                    AccountType = AccountTypeEnum.Income,
                    Month = CurrentDate.Month,
                    Year = CurrentDate.Year,
                    BudgetAmount = 150,
                    SpentAmount = 33,
                }
            };
        }

        public override IEnumerable<IEvent> Given()
        {
            yield return Ledger_Created();
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Ledger_Budget_Create(_budgets);
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Ledger_Budget_Created(_budgets);
        }

        [Test]
        public void Test()
        {
            Validate();
        }
    }
}