using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction
{
    public class invoice_void_transaction_test : TransactionTest
    {
        private readonly DateTime _date = DateTime.Now;

        public override IEnumerable<IEvent> Given()
        {
            yield return new Affiliate_CreatedEvent {Id = ApplicationId, Name = "Test Affiliate"};
            yield return new User_CreatedEvent {UserId = UserId, ApplicationId = ApplicationId};
            yield return Ledger_Created();
            yield return Ledger_Account_Added("Accounts Received", AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable);
            yield return Ledger_Account_Added("Income", AccountTypeEnum.Income, AccountLabelEnum.Income);

            yield return Transaction_Created("transaction1")
                .AddEntry("Accounts Received", 100, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable, "Income", "Income")
                .AddEntry("Income", 0, 100, CurrentDate, AccountTypeEnum.Income, AccountLabelEnum.Income, "Accounts Received", "Accounts Received");
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Transaction_Create("transaction2")
                .AddEntry("Accounts Received", 0, 100, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable, "Income", "Income")
                .AddEntry("Income", 100, 0, CurrentDate, AccountTypeEnum.Income, AccountLabelEnum.Income, "Accounts Received", "Accounts Received");
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Transaction_Created("transaction2")
                .AddEntry("Accounts Received", 0, 100, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.AccountsReceivable, "Income", "Income")
                .AddEntry("Income", 100, 0, CurrentDate, AccountTypeEnum.Income, AccountLabelEnum.Income, "Accounts Received", "Accounts Received");
        }

        [Test]
        public void Test()
        {
            DispatchEvents(()=>
                               {
                                   //// Given transaction generate these two events
                                   //yield return Ledger_Account_BalanceChanged("Accounts Received", 100);
                                   //yield return Ledger_Account_BalanceChanged("Income", 100);


                                   //yield return Ledger_Account_BalanceChanged("Accounts Received", 0);
                                   //yield return Ledger_Account_BalanceChanged("Income", 0);
                               });
            Validate();
        }
    }
}