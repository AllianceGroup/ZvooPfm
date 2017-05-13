using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction.Accounting1
{
    public class enter_rent_bill : TransactionTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return new Affiliate_CreatedEvent {Id = ApplicationId, Name = "Test Affiliate"};
            yield return new User_CreatedEvent {UserId = UserId, ApplicationId = ApplicationId};
            yield return Ledger_Created();
            yield return Ledger_Account_Added("Checking", AccountTypeEnum.Asset, AccountLabelEnum.Bank);
            yield return Ledger_Account_Added("Savings", AccountTypeEnum.Asset, AccountLabelEnum.Bank);
            yield return Ledger_Account_Added("Accounts payable", AccountTypeEnum.Liability, AccountLabelEnum.AccountsPayable);
            yield return Ledger_Account_Added("office rent", AccountTypeEnum.Expense, AccountLabelEnum.Expense);


            yield return Transaction_Created("1")
                .AddEntry("Checking", 200, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Accounts payable", "Accounts payable")
                .AddEntry("Accounts payable", 0, 200, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.AccountsPayable, "Checking", "Checking");
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            //Liability become higher, but we have paid office rent.
            yield return Transaction_Create("enter_rent_bill")
             .AddEntry("Accounts payable", 0, 100, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.AccountsPayable, "office rent", "office rent")
             .AddEntry("office rent", 100, 0, CurrentDate, AccountTypeEnum.Expense, AccountLabelEnum.Expense, "Accounts payable", "Accounts payable");
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return Transaction_Created("enter_rent_bill")
             .AddEntry("Accounts payable", 0, 100, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.AccountsPayable, "office rent", "office rent")
             .AddEntry("office rent", 100, 0, CurrentDate, AccountTypeEnum.Expense, AccountLabelEnum.Expense, "Accounts payable", "Accounts payable");
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                CheckBalance(
                 new KeyValuePair<string, long>("Accounts payable", 300),
                 new KeyValuePair<string, long>("office rent", 100));

                var _lederDocumentService = GetInstance<LedgerDocumentService>();
                var _transactionDocumentService = GetInstance<TransactionDocumentService>();
                


                var ledgerDoc = _lederDocumentService.GetById(_id);
                var transactionDoc = _transactionDocumentService.GetById("enter_rent_bill");
                var entry1 = transactionDoc.Entries.Single(x => x.AccountId == "Accounts payable");
                var entry2 = transactionDoc.Entries.Single(x => x.AccountId == "office rent");
                Assert.AreEqual(100, entry1.CreditAmountInCents);
                Assert.AreEqual(0, entry1.DebitAmountInCents);
                Assert.AreEqual(0, entry2.CreditAmountInCents);
                Assert.AreEqual(100, entry2.DebitAmountInCents);

                var account1 = ledgerDoc.Accounts.Single(x => x.Id == "Accounts payable");
                var account2 = ledgerDoc.Accounts.Single(x => x.Id == "office rent");

                Assert.AreEqual(300, account1.Denormalized.Balance);
                Assert.AreEqual(100, account2.Denormalized.Balance);

                // mobile API asserts
                //var _mobileAccountDocumentService = GetInstance<MobileAccountDocumentService>();
                //var _mobileEntryDocumentService = GetInstance<MobileEntryDocumentService>();

                //var mobileAccounts = _mobileAccountDocumentService.GetByFilter(new MobileAccountFilter { LedgerId = _id });
                //var mobileEntries = _mobileEntryDocumentService.GetByFilter(new MobileEntryFilter { TransactionId = "enter_rent_bill" });
                //var mobileEntry1 = mobileEntries.Single(x => x.AccountId == "Accounts payable");
                //var mobileEntry2 = mobileEntries.Single(x => x.AccountId == "office rent");
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(0, 100, AccountTypeEnum.Liability), mobileEntry1.AmountInDollars);
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(100, 0, AccountTypeEnum.Expense), mobileEntry2.AmountInDollars);

                //var mobileAccount1 = mobileAccounts.Single(x => x.Id == "Accounts payable");
                //var mobileAccount2 = mobileAccounts.Single(x => x.Id == "office rent");

                //Assert.AreEqual(300, mobileAccount1.Balance);
                //Assert.AreEqual(100, mobileAccount2.Balance);
            });
        }
    }
}
