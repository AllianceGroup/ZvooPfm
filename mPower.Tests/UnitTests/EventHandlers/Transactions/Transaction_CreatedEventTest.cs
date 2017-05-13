using System.Collections.Generic;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.EventHandlers.Transactions
{
    [TestFixture]
    public class Transaction_CreatedEventTest : BaseHandlerTest
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

        public override IEnumerable<IEvent> When()
        {
            yield return new Transaction_CreatedEvent
            {
                TransactionId = _id,
                BaseEntryAccountId = "4",
                BaseEntryAccountType = AccountTypeEnum.Expense,
                LedgerId = "1",
                Imported = true,
                ImportedTransactionId = "33",
                ReferenceNumber = "1",
                Type = TransactionType.BankTransfer,
                Entries = new List<ExpandedEntryData>
                {
                    new ExpandedEntryData
                    {
                        AccountId = "1",
                        AccountName = "1",
                        AccountLabel = AccountLabelEnum.Expense,
                        BookedDate = CurrentDate,
                        CreditAmountInCents = 100,
                        DebitAmountInCents = 0,
                        Memo = "Memo",
                        Payee = "Payee",
                        AccountType = AccountTypeEnum.Expense,
                        LedgerId = "1",
                        TransactionId = _id,
                        TransactionImported = true,
                        OffsetAccountId = "2",
                        OffsetAccountName = "2"
                    },
                    new ExpandedEntryData 
                    {
                        AccountId = "2",
                        AccountName = "2",
                        AccountLabel = AccountLabelEnum.Bank,
                        BookedDate = CurrentDate,
                        CreditAmountInCents = 0,
                        DebitAmountInCents = 100,
                        Memo = "Memo",
                        Payee = "Payee",
                        AccountType = AccountTypeEnum.Asset,
                        LedgerId = "1",
                        TransactionId = _id,
                        TransactionImported = true,
                        OffsetAccountId = "1",
                        OffsetAccountName = "1"
                    }
                }
            };
        }

        public override IEnumerable<object> Expected()
        {
            #region transaction event handler

            yield return new TransactionDocument
            {
                Id = _id,
                BookedDate = CurrentDate,
                BaseEntryAccountId = "4",
                BaseEntryAccountType = AccountTypeEnum.Expense,
                LedgerId = "1",
                Imported = true,
                ReferenceNumber = "1",
                Type = TransactionType.BankTransfer,
                Entries = new List<TransactionEntryDocument>
                {
                    new TransactionEntryDocument
                    {
                        AccountId = "1",
                        AccountLabel = AccountLabelEnum.Expense,
                        BookedDate = CurrentDate,
                        BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                        CreditAmountInCents = 100,
                        DebitAmountInCents = 0,
                        Memo = "Memo",
                        Payee = "Payee"
                    },
                    new TransactionEntryDocument
                    {
                        AccountId = "2",
                        AccountLabel = AccountLabelEnum.Bank,
                        BookedDate = CurrentDate,
                        BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                        CreditAmountInCents = 0,
                        DebitAmountInCents = 100,
                        Memo = "Memo",
                        Payee = "Payee"
                    }
                }
            };

            #endregion

            #region entry event hadnler

            yield return new EntryDocument
            {
                AccountId = "1",
                LedgerId = "1",
                AccountLabel = AccountLabelEnum.Expense,
                AccountName = "1",
                AccountType = AccountTypeEnum.Expense,
                BookedDate = CurrentDate,
                BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                CreditAmountInCents = 100,
                DebitAmountInCents = 0,
                FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(-100, true),
                Imported = true,
                Memo = "Memo",
                OffsetAccountId = "2",
                OffsetAccountName = "2",
                Payee = "Payee",
                TransactionId = _id,
                Id = _id + "e0"
            };

            yield return new EntryDocument
            {
                AccountId = "2",
                LedgerId = "1",
                AccountLabel = AccountLabelEnum.Bank,
                AccountName = "2",
                AccountType = AccountTypeEnum.Asset,
                BookedDate = CurrentDate,
                BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                CreditAmountInCents = 0,
                DebitAmountInCents = 100,
                FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(100, true),
                Imported = true,
                Memo = "Memo",
                OffsetAccountId = "1",
                OffsetAccountName = "1",
                Payee = "Payee",
                TransactionId = _id,
                Id = _id + "e1"
            };

            #endregion

            #region transaction statistic event handler

            yield return new TransactionsStatisticDocument
            {
                AccountId = "1",
                AccountType = AccountTypeEnum.Expense,
                CreditAmountInCents = 30100,
                DebitAmountInCents = 20000,
                Id = "1",
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year
            };

            yield return new TransactionsStatisticDocument
            {
                AccountId = "2",
                AccountType = AccountTypeEnum.Asset,
                CreditAmountInCents = 100000,
                DebitAmountInCents = 600,
                Id = "2",
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year
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
                SpentAmount = 10000 + AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(0, 100, AccountTypeEnum.Expense),
            };

            #endregion


        }

        public override IEnumerable<object> ExpectedLucene()
        {
            yield return new EntryDocument
            {
                AccountId = "2",
                LedgerId = "1",
                AccountLabel = AccountLabelEnum.Bank,
                AccountName = "2",
                AccountType = AccountTypeEnum.Asset,
                BookedDate = CurrentDate,
                CreditAmountInCents = 0,
                DebitAmountInCents = 100,
                FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(100, true),
                Memo = "Memo",
                OffsetAccountId = "1",
                OffsetAccountName = "1",
                Payee = "Payee",
                TransactionId = _id,
                Id = _id + "e1"
            };
        }

        [Test]
        public void Test()
        {
            Dispatch();
        }
    }
}
