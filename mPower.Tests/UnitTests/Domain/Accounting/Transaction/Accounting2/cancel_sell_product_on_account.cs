using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction.Accounting2
{
    class cancel_sell_product_on_account : TransactionTest
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
            yield return Ledger_Account_Added("Product sales", AccountTypeEnum.Income, AccountLabelEnum.Income);

            yield return Transaction_Created("1")
           .AddEntry("Checking", 1000, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Liability account", "Liability account")
           .AddEntry("Liability account", 0, 1000, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability, "Checking", "Checking");

            yield return Ledger_Account_BalanceChanged("Checking", 1000);
            yield return Ledger_Account_BalanceChanged("Liability account", 1000);

            yield return Transaction_Created("2")
               .AddEntry("Accounts receivable", 900, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable, "Liability account", "Liability account")
               .AddEntry("Liability account", 0, 900, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability, "Accounts receivable", "Accounts receivable");

            yield return Ledger_Account_BalanceChanged("Accounts receivable", 900);
            yield return Ledger_Account_BalanceChanged("Liability account", 1000 + 900);

        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return Transaction_Create("cancel_sell_product_on_account")
             .AddEntry("Product sales", 100, 0, CurrentDate, AccountTypeEnum.Income, AccountLabelEnum.Income, "Accounts receivable", "Accounts receivable")
             .AddEntry("Accounts receivable", 0, 100, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable, "Product sales", "Product sales");
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return Transaction_Created("cancel_sell_product_on_account")
             .AddEntry("Product sales", 100, 0, CurrentDate, AccountTypeEnum.Income, AccountLabelEnum.Income, "Accounts receivable", "Accounts receivable")
             .AddEntry("Accounts receivable", 0, 100, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable, "Product sales", "Product sales");
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                CheckBalance(
                    new KeyValuePair<string, long>("Product sales", -100),
                    new KeyValuePair<string, long>("Accounts receivable", 800));

                var _lederDocumentService = GetInstance<LedgerDocumentService>();
                var _transactionDocumentService = GetInstance<TransactionDocumentService>();

                var ledgerDoc = _lederDocumentService.GetById(_id);
                var transactionDoc = _transactionDocumentService.GetById("cancel_sell_product_on_account");
                var entry1 = transactionDoc.Entries.Single(x => x.AccountId == "Product sales");
                var entry2 = transactionDoc.Entries.Single(x => x.AccountId == "Accounts receivable");
                Assert.AreEqual(100, entry1.DebitAmountInCents);
                Assert.AreEqual(0, entry1.CreditAmountInCents);
                Assert.AreEqual(100, entry2.CreditAmountInCents);
                Assert.AreEqual(0, entry2.DebitAmountInCents);

                var account1 = ledgerDoc.Accounts.Single(x => x.Id == "Product sales");
                var account2 = ledgerDoc.Accounts.Single(x => x.Id == "Accounts receivable");

                Assert.AreEqual(-100, account1.Denormalized.Balance);
                Assert.AreEqual(800, account2.Denormalized.Balance);

                // mobile API asserts
                //var _mobileAccountDocumentService = GetInstance<MobileAccountDocumentService>();
                //var _mobileEntryDocumentService = GetInstance<MobileEntryDocumentService>();

                //var mobileAccounts = _mobileAccountDocumentService.GetByFilter(new MobileAccountFilter { LedgerId = _id });
                //var mobileEntries = _mobileEntryDocumentService.GetByFilter(new MobileEntryFilter { TransactionId = "cancel_sell_product_on_account" });
                //var mobileEntry1 = mobileEntries.Single(x => x.AccountId == "Product sales");
                //var mobileEntry2 = mobileEntries.Single(x => x.AccountId == "Accounts receivable");
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(100, 0, AccountTypeEnum.Income), mobileEntry1.AmountInDollars);
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(0, 100, AccountTypeEnum.Asset), mobileEntry2.AmountInDollars);

                //var mobileAccount1 = mobileAccounts.Single(x => x.Id == "Product sales");
                //var mobileAccount2 = mobileAccounts.Single(x => x.Id == "Accounts receivable");

                //Assert.AreEqual(-100, mobileAccount1.Balance);
                //Assert.AreEqual(800, mobileAccount2.Balance);
            });
        }
    }
}
