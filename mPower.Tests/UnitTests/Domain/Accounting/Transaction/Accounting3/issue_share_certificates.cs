using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction.Accounting3
{
    
    public class issue_share_certificates : TransactionTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return new Affiliate_CreatedEvent {Id = ApplicationId, Name = "Test Affiliate"};
            yield return new User_CreatedEvent {UserId = UserId, ApplicationId = ApplicationId};
            yield return Ledger_Created();
            yield return Ledger_Account_Added("Checking", AccountTypeEnum.Asset, AccountLabelEnum.Bank);
            yield return Ledger_Account_Added("Capital stock", AccountTypeEnum.Equity, AccountLabelEnum.Equity);
            yield return Ledger_Account_Added("Amex card", AccountTypeEnum.Liability, AccountLabelEnum.CreditCard);


            yield return Transaction_Created("1")
              .AddEntry("Checking", 300, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Amex card", "Amex card")
              .AddEntry("Amex card", 0, 300, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.CreditCard, "Checking", "Checking");

            yield return Ledger_Account_BalanceChanged("Checking", 300);
            yield return Ledger_Account_BalanceChanged("Amex card", 300);

            yield return Transaction_Created("2")
              .AddEntry("Capital stock", 1000, 0, CurrentDate, AccountTypeEnum.Equity, AccountLabelEnum.Equity, "Amex card", "Amex card")
              .AddEntry("Amex card", 0, 1000, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.CreditCard, "Capital stock", "Capital stock");

            yield return Ledger_Account_BalanceChanged("Capital stock", 1000);
            yield return Ledger_Account_BalanceChanged("Amex card", 300 + 1000);
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return Transaction_Create("issue_share_certificates")
           .AddEntry("Checking", 100, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Capital stock", "Capital stock")
           .AddEntry("Capital stock", 0, 100, CurrentDate, AccountTypeEnum.Equity, AccountLabelEnum.Equity, "Checking", "Checking");
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return Transaction_Created("issue_share_certificates")
              .AddEntry("Checking", 100, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Capital stock", "Capital stock")
              .AddEntry("Capital stock", 0, 100, CurrentDate, AccountTypeEnum.Equity, AccountLabelEnum.Equity, "Checking", "Checking");

          
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                CheckBalance(
                   new KeyValuePair<string, long>("Checking", 400),
                   new KeyValuePair<string, long>("Capital stock", 1100));

                var _lederDocumentService = GetInstance<LedgerDocumentService>();
                var _transactionDocumentService = GetInstance<TransactionDocumentService>();

                var ledgerDoc = _lederDocumentService.GetById(_id);
                var transactionDoc = _transactionDocumentService.GetById("issue_share_certificates");
                var entry1 = transactionDoc.Entries.Single(x => x.AccountId == "Checking");
                var entry2 = transactionDoc.Entries.Single(x => x.AccountId == "Capital stock");
                Assert.AreEqual(100, entry1.DebitAmountInCents);
                Assert.AreEqual(0, entry1.CreditAmountInCents);
                Assert.AreEqual(100, entry2.CreditAmountInCents);
                Assert.AreEqual(0, entry2.DebitAmountInCents);

                var account1 = ledgerDoc.Accounts.Single(x => x.Id == "Checking");
                var account2 = ledgerDoc.Accounts.Single(x => x.Id == "Capital stock");

                Assert.AreEqual(400, account1.Denormalized.Balance);
                Assert.AreEqual(1100, account2.Denormalized.Balance);

                // mobile API asserts
                //var _mobileAccountDocumentService = GetInstance<MobileAccountDocumentService>();
                //var _mobileEntryDocumentService = GetInstance<MobileEntryDocumentService>();

                //var mobileAccounts = _mobileAccountDocumentService.GetByFilter(new MobileAccountFilter {LedgerId = _id});
                //var mobileEntries = _mobileEntryDocumentService.GetByFilter(new MobileEntryFilter {TransactionId = "issue_share_certificates"});
                //var mobileEntry1 = mobileEntries.Single(x => x.AccountId == "Checking");
                //var mobileEntry2 = mobileEntries.Single(x => x.AccountId == "Capital stock");
                //Assert.AreEqual(AccountingFormatter.ConvertToDollarsThenFormat(100, 0, AccountTypeEnum.Asset), mobileEntry1.AmountInDollars);
                //Assert.AreEqual(AccountingFormatter.ConvertToDollarsThenFormat(0, 100, AccountTypeEnum.Equity), mobileEntry2.AmountInDollars);

                //var mobileAccount1 = mobileAccounts.Single(x => x.Id == "Checking");
                //var mobileAccount2 = mobileAccounts.Single(x => x.Id == "Capital stock");

                //Assert.AreEqual(400, mobileAccount1.Balance);
                //Assert.AreEqual(1100, mobileAccount2.Balance);
            });
        }
    }
}
