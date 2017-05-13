using System;
using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction
{
    public class incorrect_transaction_creation_test : TransactionTest
    {
        private readonly DateTime _date = DateTime.Now;

        public override IEnumerable<IEvent> Given()
        {
            yield return Ledger_Created();
            yield return Ledger_Account_Added("From", AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset);
            yield return Ledger_Account_Added("To", AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset);
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Transaction_Create("transaction")
                .AddEntry("From", 0, 100, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset)
                .AddEntry("To", 101, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.FixedAsset);
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield break;
        }

        [Test]
        public void Test()
        {
            AssertException<Exception>();
        }
    }
}