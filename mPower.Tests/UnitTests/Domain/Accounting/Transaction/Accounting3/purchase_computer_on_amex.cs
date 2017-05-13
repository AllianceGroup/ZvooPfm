using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction.Accounting3
{
    public class purchase_computer_on_amex : TransactionTest
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
               .AddEntry("Computer equipment", 200, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset, "Amex card", "Amex card")
               .AddEntry("Amex card", 0, 200, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.CreditCard, "Computer equipment", "Computer equipment");

            yield return Transaction_Created("2")
              .AddEntry("Computer equipment", 700, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset, "Checking", "Checking")
              .AddEntry("Checking", 0, 700, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Computer equipment", "Computer equipment");
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return Transaction_Create("purchase_computer_on_amex")
               .AddEntry("Amex card", 0, 500, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.CreditCard, "Computer equipment", "Computer equipment")
               .AddEntry("Computer equipment", 500, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset, "Amex card", "Amex card");
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return Transaction_Created("purchase_computer_on_amex")
               .AddEntry("Amex card", 0, 500, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.CreditCard, "Computer equipment", "Computer equipment")
               .AddEntry("Computer equipment", 500, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset, "Amex card", "Amex card");
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                CheckBalance(
                   new KeyValuePair<string, long>("Amex card", 700),
                   new KeyValuePair<string, long>("Computer equipment", 1400));

                var _lederDocumentService = GetInstance<LedgerDocumentService>();
                var _transactionDocumentService = GetInstance<TransactionDocumentService>();

                var ledgerDoc = _lederDocumentService.GetById(_id);
                var transactionDoc = _transactionDocumentService.GetById("purchase_computer_on_amex");
                var entry1 = transactionDoc.Entries.Single(x => x.AccountId == "Amex card");
                var entry2 = transactionDoc.Entries.Single(x => x.AccountId == "Computer equipment");
                Assert.AreEqual(0, entry1.DebitAmountInCents);
                Assert.AreEqual(500, entry1.CreditAmountInCents);
                Assert.AreEqual(0, entry2.CreditAmountInCents);
                Assert.AreEqual(500, entry2.DebitAmountInCents);

                var account1 = ledgerDoc.Accounts.Single(x => x.Id == "Amex card");
                var account2 = ledgerDoc.Accounts.Single(x => x.Id == "Computer equipment");

                Assert.AreEqual(700, account1.Denormalized.Balance);
                Assert.AreEqual(1400, account2.Denormalized.Balance);

                // mobile API asserts
                //var _mobileAccountDocumentService = GetInstance<MobileAccountDocumentService>();
                //var _mobileEntryDocumentService = GetInstance<MobileEntryDocumentService>();

                //var mobileAccounts = _mobileAccountDocumentService.GetByFilter(new MobileAccountFilter {LedgerId = _id});
                //var mobileEntries = _mobileEntryDocumentService.GetByFilter(new MobileEntryFilter {TransactionId = "purchase_computer_on_amex"});
                //var mobileEntry1 = mobileEntries.Single(x => x.AccountId == "Amex card");
                //var mobileEntry2 = mobileEntries.Single(x => x.AccountId == "Computer equipment");
                //Assert.AreEqual(AccountingFormatter.ConvertToDollarsThenFormat(0, 500, AccountTypeEnum.Liability), mobileEntry1.AmountInDollars);
                //Assert.AreEqual(AccountingFormatter.ConvertToDollarsThenFormat(500, 0, AccountTypeEnum.Asset), mobileEntry2.AmountInDollars);

                //var mobileAccount1 = mobileAccounts.Single(x => x.Id == "Amex card");
                //var mobileAccount2 = mobileAccounts.Single(x => x.Id == "Computer equipment");

                //Assert.AreEqual(700, mobileAccount1.Balance);
                //Assert.AreEqual(1400, mobileAccount2.Balance);
            });

        }
    }
}
