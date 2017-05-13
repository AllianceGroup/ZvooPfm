using System.Collections.Generic;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Domain.Accounting.Ledger.Events;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.EventHandlers.Ledger
{
    public class Ledger_Budget_SetEventTest : BaseHandlerTest
    {
        private const string UserId = "user1";

        public override IEnumerable<object> Given()
        {
            yield return new UserDocument
            {
                Id = UserId,
                ApplicationId = "application1",
            };
            yield return new LedgerDocument
            {
                Id = "1",
                Users = {new LedgerUserDocument {Id = UserId}},
            };
        }

        public override IEnumerable<IEvent> When()
        {
            yield return new Ledger_Budget_SetEvent
            {
                LedgerId = "1",
                Budgets = new List<BudgetData>
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
                },
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

            yield return new BudgetDocument
            {
                Id = "6",
                AccountId = "6",
                AccountName = "6",
                AccountType = AccountTypeEnum.Income,
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                BudgetAmount = 150,
                SpentAmount = 33,
            };
        }

        [Test]
        public void Test()
        {
            Dispatch();
        }
    }
}