using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mPower.Documents.DocumentServices.Accounting.Reports;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;

namespace mPower.Tests.UnitTests.Services
{
    public class BusinessReportDocumentServiceTest : BaseServiceTest
    {
        private BusinessReportDocumentService _service;
        private LedgerDocument _ledger;

        public override void Setup()
        {
            _ledger = new LedgerDocument
            {
                Id = _id,
                Accounts = new List<AccountDocument>
                {
                    new AccountDocument
                    {
                        Id = _id + "a0",
                        Name = "a0",
                        LabelEnum = AccountLabelEnum.Expense,
                        TypeEnum = AccountTypeEnum.Expense,
                    },
                    new AccountDocument
                    {
                        Id = _id + "a1",
                        LabelEnum = AccountLabelEnum.AccountsPayable,
                        TypeEnum = AccountTypeEnum.Liability,
                    },
                    new AccountDocument
                    {
                        Id = _id + "a1s0",
                        LabelEnum = AccountLabelEnum.AccountsPayable,
                        TypeEnum = AccountTypeEnum.Liability,
                        ParentAccountId = _id + "a1",
                    },
                },
            };

            base.Setup();

            _service = _container.GetInstance<BusinessReportDocumentService>();
        }

        public override IEnumerable<object> Given()
        {
            yield return _ledger;

            yield return new EntryDocument
            {
                Id = _id + "t0e0",
                LedgerId = _id,
                TransactionId = _id + "t0",
                AccountId = _id + "a0",
                AccountName = "a0",
                AccountLabel = AccountLabelEnum.Expense,
                AccountType = AccountTypeEnum.Expense,
                BookedDate = CurrentDate.AddDays(-5),
                BookedDateString = Format(CurrentDate.AddDays(-5)),
                CreditAmountInCents = 0,
                DebitAmountInCents = 100,
            };

            yield return new EntryDocument
            {
                Id = _id + "t1e0",
                LedgerId = _id,
                TransactionId = _id + "t1",
                AccountId = _id + "a0",
                AccountName = "a0",
                AccountLabel = AccountLabelEnum.Expense,
                AccountType = AccountTypeEnum.Expense,
                BookedDate = CurrentDate.AddDays(-2),
                BookedDateString = Format(CurrentDate.AddDays(-2)),
                CreditAmountInCents = 500,
                DebitAmountInCents = 0,
            };

            yield return new EntryDocument
            {
                Id = _id + "t2e0",
                LedgerId = _id,
                TransactionId = _id + "t2",
                AccountId = _id + "a0",
                AccountName = "a0",
                AccountLabel = AccountLabelEnum.Expense,
                AccountType = AccountTypeEnum.Expense,
                BookedDate = CurrentDate.AddDays(-2),
                BookedDateString = Format(CurrentDate.AddDays(-2)),
                CreditAmountInCents = 0,
                DebitAmountInCents = 900,
            };

            yield return new EntryDocument
            {
                Id = _id + "t2e1",
                LedgerId = _id,
                TransactionId = _id + "t2",
                AccountId = _id + "a1",
                AccountName = "a1",
                AccountLabel = AccountLabelEnum.AccountsPayable,
                AccountType = AccountTypeEnum.Liability,
                BookedDate = CurrentDate.AddDays(-2),
                BookedDateString = Format(CurrentDate.AddDays(-2)),
                CreditAmountInCents = 900,
                DebitAmountInCents = 0,
            };

            yield return new EntryDocument
            {
                Id = _id + "t3e0",
                LedgerId = _id,
                TransactionId = _id + "t3",
                AccountId = _id + "a1s0",
                AccountName = "a1s0",
                AccountLabel = AccountLabelEnum.AccountsPayable,
                AccountType = AccountTypeEnum.Liability,
                BookedDate = CurrentDate.AddDays(-7),
                BookedDateString = Format(CurrentDate.AddDays(-7)),
                CreditAmountInCents = 200,
                DebitAmountInCents = 0,
                
            };

            yield return new EntryDocument
            {
                Id = _id + "t4e0",
                LedgerId = _id,
                TransactionId = _id + "t4",
                AccountId = _id + "a1s0",
                AccountName = "a1s0",
                AccountLabel = AccountLabelEnum.AccountsPayable,
                AccountType = AccountTypeEnum.Liability,
                BookedDate = CurrentDate.AddDays(-10),
                BookedDateString = Format(CurrentDate.AddDays(-10)),
                CreditAmountInCents = 0,
                DebitAmountInCents = 150,
            };
        }

        [Test]
        public void service_correctly_builds_account_balance_by_day_with_filtering_by_date()
        {
            // filtering by date make sense only when both "from" & "to" are specified
            var accountBalances = _service.GetAccountsBalanceByDay(_ledger.Accounts, CurrentDate.AddDays(-3), CurrentDate, _ledger.Id);

            Assert.AreEqual(2, accountBalances.Count);
            Assert.AreEqual(1, accountBalances[0].AmountPerDay.Count);
            Assert.AreEqual(1, accountBalances[1].AmountPerDay.Count);
        }

        [Test]
        public void service_correctly_builds_account_balance_by_day_with_filtering_by_ledgerId()
        {
            var accountBalances = _service.GetAccountsBalanceByDay(_ledger.Accounts, null, null, _ledger.Id + "foo");

            Assert.AreEqual(0, accountBalances.Count);
        }

        [Test]
        public void service_correctly_builds_account_balance_by_day_with_filtering_by_accounts_list()
        {
            var accountBalances = _service.GetAccountsBalanceByDay(_ledger.Accounts.Where(x => x.Id == _id + "a1"), null, null, _ledger.Id);

            Assert.AreEqual(1, accountBalances.Count);
            Assert.AreEqual(_id + "a1", accountBalances[0].AccountId);
        }

        [Test]
        public void service_correctly_builds_account_balance_by_day_when_account_does_not_exist()
        {
            var accountBalances = _service.GetAccountsBalanceByDay(new[] {new AccountDocument {Id = _id + "foo"}}, null, null, _ledger.Id);

            Assert.AreEqual(0, accountBalances.Count);
        }

        [Test]
        public void service_correctly_builds_account_balance_by_day_using_one_entry()
        {
            var accountBalances = _service.GetAccountsBalanceByDay(_ledger.Accounts.Where(x => x.Id == _id + "a1"), null, null, _ledger.Id);

            Assert.AreEqual(1, accountBalances.Count);
            Assert.AreEqual(_id + "a1", accountBalances[0].AccountId);
            Assert.AreEqual(1, accountBalances[0].AmountPerDay.Count);
            var expectedAmount = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(0, 900, AccountTypeEnum.Liability);
            Assert.AreEqual(expectedAmount, accountBalances[0].AmountPerDay[0].Amount);
        }

        [Test]
        public void service_correctly_builds_account_balance_by_day_using_more_then_one_entry()
        {
            var accountBalances = _service.GetAccountsBalanceByDay(_ledger.Accounts.Where(x => x.Id == _id + "a0"), CurrentDate.AddDays(-3), CurrentDate, _ledger.Id);

            Assert.AreEqual(1, accountBalances.Count);
            Assert.AreEqual(_id + "a0", accountBalances[0].AccountId);
            Assert.AreEqual(1, accountBalances[0].AmountPerDay.Count);
            var expectedAmount = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(900, 500, AccountTypeEnum.Expense);
            Assert.AreEqual(expectedAmount, accountBalances[0].AmountPerDay[0].Amount);
        }

        [Test]
        public void service_correctly_builds_sub_account_balance_by_day_using_one_entry()
        {
            var acceptableAccountIds = new[] {_id + "a1", _id + "a1s0"};
            var accountBalances = _service.GetAccountsBalanceByDay(_ledger.Accounts.Where(x => acceptableAccountIds.Contains(x.Id)), CurrentDate.AddDays(-8), CurrentDate.AddDays(-6), _ledger.Id);

            Assert.AreEqual(1, accountBalances.Count);
            Assert.AreEqual(_id + "a1", accountBalances[0].AccountId);
            Assert.AreEqual(0, accountBalances[0].AmountPerDay.Count);
            Assert.AreEqual(1, accountBalances[0].SubAccounts.Count);
            Assert.AreEqual(_id + "a1s0", accountBalances[0].SubAccounts[0].AccountId);
            Assert.AreEqual(1, accountBalances[0].SubAccounts[0].AmountPerDay.Count);
            var expectedAmount = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(0, 200, AccountTypeEnum.Liability);
            Assert.AreEqual(expectedAmount, accountBalances[0].SubAccounts[0].AmountPerDay[0].Amount);
        }

        [Test]
        public void service_correctly_builds_sub_account_balance_by_day_using_more_then_one_entry()
        {
            var acceptableAccountIds = new[] {_id + "a1", _id + "a1s0"};
            var accountBalances = _service.GetAccountsBalanceByDay(_ledger.Accounts.Where(x => acceptableAccountIds.Contains(x.Id)), CurrentDate.AddDays(-11), CurrentDate.AddDays(-6), _ledger.Id);

            Assert.AreEqual(1, accountBalances.Count);
            Assert.AreEqual(_id + "a1", accountBalances[0].AccountId);
            Assert.AreEqual(0, accountBalances[0].AmountPerDay.Count);
            Assert.AreEqual(1, accountBalances[0].SubAccounts.Count);
            Assert.AreEqual(_id + "a1s0", accountBalances[0].SubAccounts[0].AccountId);
            Assert.AreEqual(2, accountBalances[0].SubAccounts[0].AmountPerDay.Count);
            var expectedAmount = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(150, 200, AccountTypeEnum.Liability);
            Assert.AreEqual(expectedAmount, accountBalances[0].SubAccounts[0].AmountPerDay.Sum(x => x.Amount));
        }

        [Test]
        public void service_correctly_builds_profit_loss_report_data()
        {
            var data = _service.GetProfitLossReportData(null, null, _ledger);

            Assert.AreEqual(1, data.Count);
            Assert.IsTrue(data.All(IsIncomeOrExpense));
        }

        private static bool IsIncomeOrExpense(LedgerAccountBalanceByDay data)
        {
            var acceptableTypes = new[] {AccountTypeEnum.Expense, AccountTypeEnum.Income};
            return acceptableTypes.Contains(data.AccountType) && data.SubAccounts.All(IsIncomeOrExpense);
        }

        private static string Format(DateTime date)
        {
            return date.ToString("MM-dd-yyyy");
        }
    }
}