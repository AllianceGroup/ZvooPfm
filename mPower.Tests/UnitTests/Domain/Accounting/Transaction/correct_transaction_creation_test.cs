using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction
{
    public class correct_transaction_creation_test : TransactionTest
    {
        private readonly DateTime _date = DateTime.Now;

        public override IEnumerable<IEvent> Given()
        {
            yield return new Affiliate_CreatedEvent {Id = ApplicationId, Name = "Test Affiliate"};
            yield return new User_CreatedEvent {UserId = UserId, ApplicationId = ApplicationId};
            yield return Ledger_Created();
            yield return Ledger_Account_Added("Checking", AccountTypeEnum.Asset, AccountLabelEnum.Bank);
            yield return Ledger_Account_Added("Savings", AccountTypeEnum.Asset, AccountLabelEnum.Bank);
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Transaction_Create("transaction")
                .AddEntry("Checking", 0, 100, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Savings", "Savings")
                .AddEntry("Savings", 100, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Checking", "Checking");
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Transaction_Created("transaction")
                .AddEntry("Checking", 0, 100, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Savings", "Savings")
                .AddEntry("Savings", 100, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Checking", "Checking");
        }

        [Test]
        public void Test()
        {
            DispatchEvents(()=>
                               {
                                   //yield return Ledger_Account_BalanceChanged("Checking", -100);
                                   //yield return Ledger_Account_BalanceChanged("Savings", 100);
                               });
            Validate();
        }
    }
}