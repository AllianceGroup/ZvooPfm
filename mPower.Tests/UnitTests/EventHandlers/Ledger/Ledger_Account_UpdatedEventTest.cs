using System.Collections.Generic;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Tests.Environment;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.EventHandlers.Ledger
{
    public class Ledger_Account_UpdatedEventTest : BaseHandlerTest
    {
        public override IEnumerable<object> Given()
        {
            yield return new LedgerDocument
            {
                Id = _id,
                Name = "Test ledger",
                Accounts = new List<AccountDocument>
                {
                    CreateAccountDocument("1"),
                    CreateAccountDocument("2", type: AccountTypeEnum.Equity),
                    CreateAccountDocument("3", type: AccountTypeEnum.Expense),
                    CreateAccountDocument("4", type: AccountTypeEnum.Income),
                    CreateAccountDocument("5", type: AccountTypeEnum.Liability),
                    CreateAccountDocument("51", parentId: "5"),
                    CreateAccountDocument("6"),
                    CreateAccountDocument("7"),
                    CreateAccountDocument("8"),
                    CreateAccountDocument("9"),
                    CreateAccountDocument("10"),
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
                        AccountId = "51",
                        AccountType = AccountTypeEnum.Expense,
                        AccountName = "51",
                        SpentAmount = 20,
                        ParentAccountId = "5",
                    }
                },
            };
        }

        private static AccountDocument CreateAccountDocument(string id, string name = "Test account", string description = "Test description", AccountTypeEnum type = AccountTypeEnum.Asset, string number = "1234", string parentId = null)
        {
            return new AccountDocument
            {
                Id = id,
                Name = name,
                Description = description,
                TypeEnum = type,
                Number = number,
                ParentAccountId = parentId,
            };
        }

        public override IEnumerable<IEvent> When()
        {
            yield return new Ledger_Account_UpdatedEvent
            {
                LedgerId = _id,
                AccountId = "5",
                Name = "5u",
                Description = "5",
                Number = "5",
                ParentAccountId = null,
            };

            yield return new Ledger_Account_UpdatedEvent
            {
                LedgerId = _id,
                AccountId = "51",
                Name = "51u",
                Description = "51",
                Number = "51",
                ParentAccountId = "5",
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
                    CreateAccountDocument("1"),
                    CreateAccountDocument("2", type: AccountTypeEnum.Equity),
                    CreateAccountDocument("3", type: AccountTypeEnum.Expense),
                    CreateAccountDocument("4", type: AccountTypeEnum.Income),
                    CreateAccountDocument("5", "5u", "5", AccountTypeEnum.Liability, "5", null),
                    CreateAccountDocument("51", "51u", "51", number: "51", parentId: "5"),
                    CreateAccountDocument("6"),
                    CreateAccountDocument("7"),
                    CreateAccountDocument("8"),
                    CreateAccountDocument("9"),
                    CreateAccountDocument("10"),
                }
            };

            #endregion

            #region budget event handler

            yield return new BudgetDocument
            {
                Id = "5",
                LedgerId = _id,
                AccountId = "5",
                AccountName = "5u",
                AccountType = AccountTypeEnum.Expense,
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
                        AccountName = "51u",
                        SpentAmount = 20,
                        ParentAccountId = "5",
                    }
                },
            };

            #endregion
        }

        [Test]
        public void Test()
        {
            Dispatch(ignoreList: IgnoreList.Create("DateLastAggregated"));
        }
    }
}