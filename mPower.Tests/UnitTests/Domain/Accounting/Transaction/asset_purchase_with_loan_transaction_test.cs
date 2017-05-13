using System.Collections.Generic;
using System.Linq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction
{
    public class asset_purchase_with_loan_transaction_test : TransactionTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield return new Affiliate_CreatedEvent {Id = ApplicationId, Name = "Test Affiliate"};
            yield return new User_CreatedEvent {UserId = UserId, ApplicationId = ApplicationId};
            yield return Ledger_Created();
            yield return Ledger_Account_Added("Checking", AccountTypeEnum.Asset, AccountLabelEnum.Bank);
            yield return Ledger_Account_Added("Car Loan", AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability);
            yield return Ledger_Account_Added("Car Asset", AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset);
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Transaction_Create("transaction")
                .AddEntry("Checking", 0, 50, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Split", "Split")
                .AddEntry("Car Loan", 0, 50, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability, "Split", "Split")
                .AddEntry("Car Asset", 100, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset, "Split", "Split");
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Transaction_Created("transaction")
                .AddEntry("Checking", 0, 50, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Split", "Split")
                .AddEntry("Car Loan", 0, 50, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability, "Split", "Split")
                .AddEntry("Car Asset", 100, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset, "Split", "Split");
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var ledgerService = _container.GetInstance<LedgerDocumentService>();
                var ledger = ledgerService.GetById(_id);
                Assert.AreEqual(-50, ledger.Accounts.Single(x => x.Id == "Checking").Denormalized.Balance);
                Assert.AreEqual(50, ledger.Accounts.Single(x => x.Id == "Car Loan").Denormalized.Balance);
                Assert.AreEqual(100, ledger.Accounts.Single(x => x.Id == "Car Asset").Denormalized.Balance);
            });
        }
    }

}