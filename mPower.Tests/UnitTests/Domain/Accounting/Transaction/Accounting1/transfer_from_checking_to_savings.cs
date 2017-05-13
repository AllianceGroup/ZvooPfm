using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction.Accounting1
{
    public class transfer_from_checking_to_savings : TransactionTest
    {
        private readonly DateTime _date = DateTime.Now;

        public override IEnumerable<IEvent> Given()
        {
            yield return new Affiliate_CreatedEvent {Id = ApplicationId, Name = "Test Affiliate"};
            yield return new User_CreatedEvent {UserId = UserId, ApplicationId = ApplicationId};

            #region Ledger & Accounts

            yield return Ledger_Created();
            yield return Ledger_Account_Added("1", AccountTypeEnum.Asset, AccountLabelEnum.Bank); // "Checking"
            yield return Ledger_Account_Added("2", AccountTypeEnum.Asset, AccountLabelEnum.Bank); // "Savings"
            yield return Ledger_Account_Added("3", AccountTypeEnum.Liability, AccountLabelEnum.AccountsPayable); //"Accounts payable"

            #endregion

            #region Transaction 1

            yield return Transaction_Created("1")
                .AddEntry("1", 800, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank,"3","3")
                .AddEntry("3", 0, 800, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank,"1","1");

            yield return Ledger_Account_BalanceChanged("1", 800);
            yield return Ledger_Account_BalanceChanged("3", 800);

            #endregion

            #region Transaction 2

            yield return Transaction_Created("2")
                .AddEntry("2", 100, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank,"3", "3")
                .AddEntry("3", 0, 100, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank,"2", "2");

            yield return Ledger_Account_BalanceChanged("2", 100);
            yield return Ledger_Account_BalanceChanged("3", 800 + 100);

            #endregion
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Transaction_Create("3")
                .AddEntry("2", 400, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "1", "1")
                .AddEntry("1", 0, 400, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "2", "2");
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Transaction_Created("3")
                .AddEntry("2", 400, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "1", "1")
                .AddEntry("1", 0, 400, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank,"2", "2");
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                CheckBalance(
                  new KeyValuePair<string, long>("2", 500),
                  new KeyValuePair<string, long>("1", 400));

                var _lederDocumentService = GetInstance<LedgerDocumentService>();
                var _transactionDocumentService = GetInstance<TransactionDocumentService>();

                var ledgerDoc = _lederDocumentService.GetById(_id);
                var transactionDoc = _transactionDocumentService.GetById("3");
                var entry1 = transactionDoc.Entries.Single(x => x.AccountId == "1");
                var entry2 = transactionDoc.Entries.Single(x => x.AccountId == "2");
                Assert.AreEqual(400, entry1.CreditAmountInCents);
                Assert.AreEqual(0, entry1.DebitAmountInCents);
                Assert.AreEqual(400, entry2.DebitAmountInCents);
                Assert.AreEqual(0, entry2.CreditAmountInCents);

                var account1 = ledgerDoc.Accounts.Single(x => x.Id == "1");
                var account2 = ledgerDoc.Accounts.Single(x => x.Id == "2");

                Assert.AreEqual(400, account1.Denormalized.Balance);
                Assert.AreEqual(500, account2.Denormalized.Balance);

                // mobile API asserts
                //var _mobileAccountDocumentService = GetInstance<MobileAccountDocumentService>();
                //var _mobileEntryDocumentService = GetInstance<MobileEntryDocumentService>();

                //var mobileAccounts = _mobileAccountDocumentService.GetByFilter(new MobileAccountFilter { LedgerId = _id });
                //var mobileEntries = _mobileEntryDocumentService.GetByFilter(new MobileEntryFilter { TransactionId = "3" });
                //var mobileEntry1 = mobileEntries.Single(x => x.AccountId == "1");
                //var mobileEntry2 = mobileEntries.Single(x => x.AccountId == "2");
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(0, 400, AccountTypeEnum.Asset), mobileEntry1.AmountInDollars);
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(400, 0, AccountTypeEnum.Asset), mobileEntry2.AmountInDollars);

                //var mobileAccount1 = mobileAccounts.Single(x => x.Id == "1");
                //var mobileAccount2 = mobileAccounts.Single(x => x.Id == "2");

                //Assert.AreEqual(400, mobileAccount1.Balance);
                //Assert.AreEqual(500, mobileAccount2.Balance);
            });
        }
    }
}
