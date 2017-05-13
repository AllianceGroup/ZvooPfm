using System.Collections.Generic;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Tests.Environment;

namespace mPower.Tests.UnitTests.EventHandlers.Ledger
{
    public class Ledger_Account_RemovedEventTest : BaseHandlerTest
    {
        public override IEnumerable<object> Given()
        {
            yield return new LedgerDocument
            {
                Id = _id,
                Name = "Test ledger",
                Accounts = new List<AccountDocument>
                {
                    CreateAccountDocument("a1"),
                    CreateAccountDocument("a2"),
                    CreateAccountDocument("a3"),
                    CreateAccountDocument("a4"),
                    CreateAccountDocument("a5"),
                    CreateAccountDocument("a6"),
                    CreateAccountDocument("a7"),
                    CreateAccountDocument("a8"),
                    CreateAccountDocument("a9"),
                    CreateAccountDocument("a10"),
                }
            };

            yield return new BudgetDocument
            {
                Id = "5",
                AccountId = "5",
                AccountName = "5",
                AccountType = AccountTypeEnum.Expense,
                LedgerId = _id,
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                BudgetAmount = 100,
                SpentAmount = 20,
                SubBudgets = new List<ChildBudgetDocument>
                {
                    new ChildBudgetDocument
                    {
                        AccountId = "a3",
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
                AccountId = "a4",
                AccountName = "6",
                AccountType = AccountTypeEnum.Income,
                LedgerId = _id,
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                BudgetAmount = 150,
                SpentAmount = 33,
            };
        }

        private static AccountDocument CreateAccountDocument(string id, string name = "Test account", AccountTypeEnum type = AccountTypeEnum.Asset)
        {
            return new AccountDocument
            {
                Id = id,
                Name = name + " " + type,
                TypeEnum = type
            };
        }

        public override IEnumerable<IEvent> When()
        {
            yield return new Ledger_Account_RemovedEvent
            {
                LedgerId = _id,
                AccountId = "a3",
                Label = AccountLabelEnum.Expense,
            };

            yield return new Ledger_Account_RemovedEvent
            {
                LedgerId = _id,
                AccountId = "a4",
                Label = AccountLabelEnum.Income,
            };

            yield return new Ledger_Account_RemovedEvent
            {
                LedgerId = _id,
                AccountId = "a1"
            };
        }

        public override IEnumerable<object> Expected()
        {
            #region ledger event handler

            yield return new LedgerDocument
            {
                Id = _id,
                Name = "Test ledger",
                Accounts = new List<AccountDocument>
                {
                    CreateAccountDocument("a2"),
                    CreateAccountDocument("a5"),
                    CreateAccountDocument("a6"),
                    CreateAccountDocument("a7"),
                    CreateAccountDocument("a8"),
                    CreateAccountDocument("a9"),
                    CreateAccountDocument("a10"),
                }
            };

            #endregion

            #region budget event handler

            yield return new BudgetDocument
            {
                Id = "5",
                AccountId = "5",
                AccountName = "5",
                AccountType = AccountTypeEnum.Expense,
                LedgerId = _id,
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                BudgetAmount = 100,
                SpentAmount = 20,
            };

            #endregion
        }

        public override IEnumerable<object> ShouldBeDeleted()
        {
            yield return new BudgetDocument
            {
                Id = "6",
            };
        }

        [Ignore]
        [Test]
        public void Test()
        {
            Dispatch(ignoreList: IgnoreList.Create("DateLastAggregated"));
        }
    }
}