using System.Collections.Generic;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Transaction.Events;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.EventHandlers.Transactions
{
    [TestFixture]
    public class Transaction_DeletedEventTest : BaseHandlerTest
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

            yield return new TransactionDocument
            {
                Id = _id,
                LedgerId = "1"
            };

            yield return new EntryDocument
            {
                Id = _id + "e0",
                TransactionId = _id,
                LedgerId = "1",
                AccountId = "1",
                AccountLabel = AccountLabelEnum.Expense,
                AccountType = AccountTypeEnum.Expense,
                BookedDate = CurrentDate,
                BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                CreditAmountInCents = 100,
                DebitAmountInCents = 0,
                Memo = "Memo",
                Payee = "Payee"
            };

            yield return new EntryDocument
            {
                Id = _id+"e1",
                TransactionId = _id,
                LedgerId = "1",
                AccountId = "2",
                AccountLabel = AccountLabelEnum.Bank,
                AccountType = AccountTypeEnum.Asset,
                BookedDate = CurrentDate,
                BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                CreditAmountInCents = 0,
                DebitAmountInCents = 100,
                Memo = "Memo",
                Payee = "Payee"
            };

            yield return new TransactionsStatisticDocument
            {
                AccountId = "1",
                AccountType = AccountTypeEnum.Expense,
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                Id = "1",
                CreditAmountInCents = 30000,
                DebitAmountInCents = 20000
            };

            yield return new TransactionsStatisticDocument
            {
                AccountId = "2",
                AccountType = AccountTypeEnum.Asset,
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                Id = "2",
                CreditAmountInCents = 100000,
                DebitAmountInCents = 500
            };

            yield return new BudgetDocument
            {
                Id = "1",
                AccountId = "1",
                AccountName = "1",
                AccountType = AccountTypeEnum.Expense,
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                BudgetAmount = 20000,
                SpentAmount = 10000,
            };
        }

        public override IEnumerable<object> GivenLucene()
        {
            yield return new EntryDocument
            {
                Id = _id + "e1",
                TransactionId = _id,
                LedgerId = "1",
                AccountId = "2",
                AccountLabel = AccountLabelEnum.Bank,
                AccountType = AccountTypeEnum.Asset,
                BookedDate = CurrentDate,
                BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                CreditAmountInCents = 0,
                DebitAmountInCents = 100,
                Memo = "Memo",
                Payee = "Payee"
            };
        }

        public override IEnumerable<IEvent> When()
        {
            yield return new Transaction_DeletedEvent
            {
                TransactionId = _id,
                LedgerId = "1"  
            };
        }

        public override IEnumerable<object> Expected()
        {
            #region transaction statistic event handler

            yield return new TransactionsStatisticDocument
            {
                AccountId = "1",
                AccountType = AccountTypeEnum.Expense,
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                Id = "1",
                CreditAmountInCents = 29900,
                DebitAmountInCents = 20000
            };

            yield return new TransactionsStatisticDocument
            {
                AccountId = "2",
                AccountType = AccountTypeEnum.Asset,
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                Id = "2",
                CreditAmountInCents = 100000,
                DebitAmountInCents = 400
            };

            #endregion

            #region budget event handler

            yield return new BudgetDocument
            {
                Id = "1",
                AccountId = "1",
                AccountName = "1",
                AccountType = AccountTypeEnum.Expense,
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                BudgetAmount = 20000,
                SpentAmount = 10000 + AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(0, -100, AccountTypeEnum.Expense),
            };

            #endregion
        }

        public override IEnumerable<object> ShouldBeDeleted()
        {
            yield return new TransactionDocument
            {
                Id = _id,
            };
            yield return new EntryDocument
            {
                Id = _id + "e0",
            };
            yield return new EntryDocument
            {
                Id = _id + "e1",
            };
        }

        public override IEnumerable<object> ShouldBeDeletedLucene()
        {
            yield return new EntryDocument
            {
                Id = _id + "e1",
            };
        }

        [Test]
        public void Test()
        {
            Dispatch();
        }
    }
}