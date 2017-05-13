using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction.Accounting3
{
    public class purchase_treasury_shares : TransactionTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return new Affiliate_CreatedEvent {Id = ApplicationId, Name = "Test Affiliate"};
            yield return new User_CreatedEvent {UserId = UserId, ApplicationId = ApplicationId};
            yield return Ledger_Created();
            yield return Ledger_Account_Added("Checking", AccountTypeEnum.Asset, AccountLabelEnum.Bank);
            yield return Ledger_Account_Added("Capital stock", AccountTypeEnum.Equity, AccountLabelEnum.Equity);
            yield return Ledger_Account_Added("Treasury stock", AccountTypeEnum.Equity, AccountLabelEnum.Equity);
            yield return Ledger_Account_Added("Amex card", AccountTypeEnum.Liability, AccountLabelEnum.CreditCard);
            yield return Ledger_Account_Added("Liability", AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability);
            yield return Ledger_Account_Added("Computer equipment", AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset);

            yield return Transaction_Created("1")
               .AddEntry("Checking", 400, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Amex card", "Amex card")
               .AddEntry("Amex card", 0, 400, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.CreditCard, "Checking", "Checking");

            yield return Ledger_Account_BalanceChanged("Checking", 400);
            yield return Ledger_Account_BalanceChanged("Amex card", 400);

            yield return Transaction_Created("2")
              .AddEntry("Capital stock", 1100, 0, CurrentDate, AccountTypeEnum.Equity, AccountLabelEnum.Equity, "Amex card", "Amex card")
              .AddEntry("Amex card", 0, 1100, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.CreditCard, "Capital stock", "Capital stock");

            yield return Ledger_Account_BalanceChanged("Capital stock", 1100);
            yield return Ledger_Account_BalanceChanged("Amex card", 400 + 1100);
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return Transaction_Create("purchase_treasury_shares")
                .AddEntry("Checking", 0, 50, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Treasury stock", "Treasury stock")
                .AddEntry("Treasury stock", 50, 0, CurrentDate, AccountTypeEnum.Equity, AccountLabelEnum.Equity, "Checking", "Checking");
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return Transaction_Created("purchase_treasury_shares")
                .AddEntry("Checking", 0, 50, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Treasury stock", "Treasury stock")
                .AddEntry("Treasury stock", 50, 0, CurrentDate, AccountTypeEnum.Equity, AccountLabelEnum.Equity, "Checking", "Checking");
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                CheckBalance(
                 new KeyValuePair<string, long>("Checking", 350),
                 new KeyValuePair<string, long>("Treasury stock", -50));

                var _lederDocumentService = GetInstance<LedgerDocumentService>();
                var _transactionDocumentService = GetInstance<TransactionDocumentService>();

                var ledgerDoc = _lederDocumentService.GetById(_id);
                var transactionDoc = _transactionDocumentService.GetById("purchase_treasury_shares");
                var entry1 = transactionDoc.Entries.Single(x => x.AccountId == "Checking");
                var entry2 = transactionDoc.Entries.Single(x => x.AccountId == "Treasury stock");
                Assert.AreEqual(0, entry1.DebitAmountInCents);
                Assert.AreEqual(50, entry1.CreditAmountInCents);
                Assert.AreEqual(0, entry2.CreditAmountInCents);
                Assert.AreEqual(50, entry2.DebitAmountInCents);

                var account1 = ledgerDoc.Accounts.Single(x => x.Id == "Checking");
                var account2 = ledgerDoc.Accounts.Single(x => x.Id == "Treasury stock");

                Assert.AreEqual(350, account1.Denormalized.Balance);
                Assert.AreEqual(-50, account2.Denormalized.Balance);

                // mobile API asserts
                //var _mobileAccountDocumentService = GetInstance<MobileAccountDocumentService>();
                //var _mobileEntryDocumentService = GetInstance<MobileEntryDocumentService>();

                //var mobileAccounts = _mobileAccountDocumentService.GetByFilter(new MobileAccountFilter {LedgerId = _id});
                //var mobileEntries = _mobileEntryDocumentService.GetByFilter(new MobileEntryFilter {TransactionId = "purchase_treasury_shares"});
                //var mobileEntry1 = mobileEntries.Single(x => x.AccountId == "Checking");
                //var mobileEntry2 = mobileEntries.Single(x => x.AccountId == "Treasury stock");
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(0, 50, AccountTypeEnum.Asset), mobileEntry1.AmountInDollars);
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(50, 0, AccountTypeEnum.Equity), mobileEntry2.AmountInDollars);

                //var mobileAccount1 = mobileAccounts.Single(x => x.Id == "Checking");
                //var mobileAccount2 = mobileAccounts.Single(x => x.Id == "Treasury stock");

                //Assert.AreEqual(350, mobileAccount1.Balance);
                //Assert.AreEqual(-50, mobileAccount2.Balance);
            });

        }
    }
}
