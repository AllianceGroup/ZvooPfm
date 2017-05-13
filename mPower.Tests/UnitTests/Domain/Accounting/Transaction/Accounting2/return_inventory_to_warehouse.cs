using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction.Accounting2
{
    public class return_inventory_to_warehouse : TransactionTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return new Affiliate_CreatedEvent {Id = ApplicationId, Name = "Test Affiliate"};
            yield return new User_CreatedEvent {UserId = UserId, ApplicationId = ApplicationId};
            yield return Ledger_Created();
            yield return Ledger_Account_Added("Cost of goods sold", AccountTypeEnum.Expense, AccountLabelEnum.CostOfGoodsSold);
            yield return Ledger_Account_Added("Accounts receivable", AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable);
            yield return Ledger_Account_Added("Inventory", AccountTypeEnum.Asset, AccountLabelEnum.OtherCurrentAsset);
            yield return Ledger_Account_Added("Liability account", AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability);
            yield return Ledger_Account_Added("Product sales", AccountTypeEnum.Income, AccountLabelEnum.Income);

            yield return Transaction_Created("1")
              .AddEntry("Inventory", 450, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.OtherCurrentAsset, "Liability account", "Liability account")
              .AddEntry("Liability account", 0, 450, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability,"Inventory","Inventory");
            
            yield return Ledger_Account_BalanceChanged("Inventory", 450);
            yield return Ledger_Account_BalanceChanged("Liability account", 450);
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return Transaction_Create("return_inventory_to_warehouse")
             .AddEntry("Inventory", 50, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.OtherCurrentAsset, "Cost of goods sold", "Cost of goods sold")
             .AddEntry("Cost of goods sold", 0, 50, CurrentDate, AccountTypeEnum.Expense, AccountLabelEnum.CostOfGoodsSold, "Inventory", "Inventory");
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return Transaction_Created("return_inventory_to_warehouse")
              .AddEntry("Inventory", 50, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.OtherCurrentAsset, "Cost of goods sold","Cost of goods sold")
              .AddEntry("Cost of goods sold", 0, 50, CurrentDate, AccountTypeEnum.Expense, AccountLabelEnum.CostOfGoodsSold, "Inventory", "Inventory");

           
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                CheckBalance(
                    new KeyValuePair<string, long>("Inventory", 500),
                    new KeyValuePair<string, long>("Cost of goods sold", -50));

                var _lederDocumentService = GetInstance<LedgerDocumentService>();
                var _transactionDocumentService = GetInstance<TransactionDocumentService>();

                var ledgerDoc = _lederDocumentService.GetById(_id);
                var transactionDoc = _transactionDocumentService.GetById("return_inventory_to_warehouse");
                var entry1 = transactionDoc.Entries.Single(x => x.AccountId == "Inventory");
                var entry2 = transactionDoc.Entries.Single(x => x.AccountId == "Cost of goods sold");
                Assert.AreEqual(50, entry1.DebitAmountInCents);
                Assert.AreEqual(0, entry1.CreditAmountInCents);
                Assert.AreEqual(50, entry2.CreditAmountInCents);
                Assert.AreEqual(0, entry2.DebitAmountInCents);

                var account1 = ledgerDoc.Accounts.Single(x => x.Id == "Inventory");
                var account2 = ledgerDoc.Accounts.Single(x => x.Id == "Cost of goods sold");

                Assert.AreEqual(500, account1.Denormalized.Balance);
                Assert.AreEqual(-50, account2.Denormalized.Balance);

                // mobile API asserts
                //var _mobileAccountDocumentService = GetInstance<MobileAccountDocumentService>();
                //var _mobileEntryDocumentService = GetInstance<MobileEntryDocumentService>();

                //var mobileAccounts = _mobileAccountDocumentService.GetByFilter(new MobileAccountFilter {LedgerId = _id});
                //var mobileEntries = _mobileEntryDocumentService.GetByFilter(new MobileEntryFilter {TransactionId = "return_inventory_to_warehouse"});
                //var mobileEntry1 = mobileEntries.Single(x => x.AccountId == "Inventory");
                //var mobileEntry2 = mobileEntries.Single(x => x.AccountId == "Cost of goods sold");
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(50, 0, AccountTypeEnum.Asset), mobileEntry1.AmountInDollars);
                //Assert.AreEqual(AccountingFormatter.CentsToDollarString(0, 50, AccountTypeEnum.Expense), mobileEntry2.AmountInDollars);

                //var mobileAccount1 = mobileAccounts.Single(x => x.Id == "Inventory");
                //var mobileAccount2 = mobileAccounts.Single(x => x.Id == "Cost of goods sold");

                //Assert.AreEqual(500, mobileAccount1.Balance);
                //Assert.AreEqual(-50, mobileAccount2.Balance);
            });
        }
    }
}
