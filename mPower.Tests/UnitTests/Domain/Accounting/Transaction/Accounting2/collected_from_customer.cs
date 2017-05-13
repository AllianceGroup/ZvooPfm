using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction.Accounting2
{
    public class collected_from_customer : TransactionTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return new Affiliate_CreatedEvent {Id = ApplicationId, Name = "Test Affiliate"};
            yield return new User_CreatedEvent {UserId = UserId, ApplicationId = ApplicationId};
            yield return Ledger_Created();
            yield return Ledger_Account_Added("Checking", AccountTypeEnum.Asset, AccountLabelEnum.Bank);
            yield return Ledger_Account_Added("Accounts receivable", AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable);
            yield return Ledger_Account_Added("Inventory", AccountTypeEnum.Asset, AccountLabelEnum.OtherCurrentAsset);
            yield return Ledger_Account_Added("Liability account", AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability);

            yield return Transaction_Created("1")
               .AddEntry("Checking", 600, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Liability account", "Liability account")
               .AddEntry("Liability account", 0, 600, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability, "Checking", "Checking");

            yield return Ledger_Account_BalanceChanged("Checking", 600);
            yield return Ledger_Account_BalanceChanged("Liability account", 200);

            yield return Transaction_Created("2")
               .AddEntry("Accounts receivable", 300, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable, "Liability account", "Liability account")
               .AddEntry("Liability account", 0, 300, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability, "Accounts receivable", "Accounts receivable");

            yield return Ledger_Account_BalanceChanged("Accounts receivable", 300);
            yield return Ledger_Account_BalanceChanged("Liability account", 900);
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return Transaction_Create("collected_from_customer")
            .AddEntry("Checking", 50, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Accounts receivable", "Accounts receivable")
            .AddEntry("Accounts receivable", 0, 50, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable, "Checking", "Checking");
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return Transaction_Created("collected_from_customer")
            .AddEntry("Checking", 50, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Accounts receivable", "Accounts receivable")
            .AddEntry("Accounts receivable", 0, 50, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable, "Checking", "Checking");

          
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                CheckBalance(
                   new KeyValuePair<string, long>("Checking", 650),
                   new KeyValuePair<string, long>("Accounts receivable", 250));

                var _lederDocumentService = GetInstance<LedgerDocumentService>();
                var _transactionDocumentService = GetInstance<TransactionDocumentService>();

                var ledgerDoc = _lederDocumentService.GetById(_id);
                var transactionDoc = _transactionDocumentService.GetById("collected_from_customer");
                var entry1 = transactionDoc.Entries.Single(x => x.AccountId == "Checking");
                var entry2 = transactionDoc.Entries.Single(x => x.AccountId == "Accounts receivable");
                Assert.AreEqual(50, entry1.DebitAmountInCents);
                Assert.AreEqual(0, entry1.CreditAmountInCents);
                Assert.AreEqual(50, entry2.CreditAmountInCents);
                Assert.AreEqual(0, entry2.DebitAmountInCents);

                var account1 = ledgerDoc.Accounts.Single(x => x.Id == "Checking");
                var account2 = ledgerDoc.Accounts.Single(x => x.Id == "Accounts receivable");

                Assert.AreEqual(650, account1.Denormalized.Balance);
                Assert.AreEqual(250, account2.Denormalized.Balance);

                // mobile API asserts
                //var _mobileAccountDocumentService = GetInstance<MobileAccountDocumentService>();
                //var _mobileEntryDocumentService = GetInstance<MobileEntryDocumentService>();

                //var mobileAccounts = _mobileAccountDocumentService.GetByFilter(new MobileAccountFilter {LedgerId = _id});
                //var mobileEntries = _mobileEntryDocumentService.GetByFilter(new MobileEntryFilter {TransactionId = "collected_from_customer"});
                //var mobileEntry1 = mobileEntries.Single(x => x.AccountId == "Checking");
                //var mobileEntry2 = mobileEntries.Single(x => x.AccountId == "Accounts receivable");
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(50, 0, AccountTypeEnum.Asset), mobileEntry1.AmountInDollars);
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(0, 50, AccountTypeEnum.Asset), mobileEntry2.AmountInDollars);

                //var mobileAccount1 = mobileAccounts.Single(x => x.Id == "Checking");
                //var mobileAccount2 = mobileAccounts.Single(x => x.Id == "Accounts receivable");

                //Assert.AreEqual(650, mobileAccount1.Balance);
                //Assert.AreEqual(250, mobileAccount2.Balance);
            });
        }
    }
}
