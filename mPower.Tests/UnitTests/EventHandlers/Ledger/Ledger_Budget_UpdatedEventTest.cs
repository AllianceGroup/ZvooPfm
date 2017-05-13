using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;

namespace mPower.Tests.UnitTests.EventHandlers.Ledger
{
    public class Ledger_Budget_UpdatedEventTest : BaseHandlerTest
    {
        public override IEnumerable<object> Given()
        {
            yield return new BudgetDocument
            {
                Id = "5",
                AccountId = "5",
                AccountName = "5",
                AccountType = AccountTypeEnum.Expense,
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                BudgetAmount = 100,
                SpentAmount = 20,
                SubBudgets = new List<ChildBudgetDocument>
                {
                    new ChildBudgetDocument
                    {
                        AccountId = "51",
                        AccountType = AccountTypeEnum.Expense,
                        AccountName = "51",
                        SpentAmount = 20,
                        ParentAccountId = "5",
                    }
                },
            };
        }

        public override IEnumerable<IEvent> When()
        {
            yield return new Ledger_Budget_UpdatedEvent
            {
                LedgerId = "1",
                BudgetId = "5",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                Amount = 200,
            };
        }

        public override IEnumerable<object> Expected()
        {
            yield return new BudgetDocument
            {
                Id = "5",
                AccountId = "5",
                AccountName = "5",
                AccountType = AccountTypeEnum.Expense,
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                BudgetAmount = 200,
                SpentAmount = 20,
                SubBudgets = new List<ChildBudgetDocument>
                {
                    new ChildBudgetDocument
                    {
                        AccountId = "51",
                        AccountType = AccountTypeEnum.Expense,
                        AccountName = "51",
                        SpentAmount = 20,
                        ParentAccountId = "5",
                    }
                },
            };
        }

        [Test]
        public void Test()
        {
            Dispatch();
        }
    }
}