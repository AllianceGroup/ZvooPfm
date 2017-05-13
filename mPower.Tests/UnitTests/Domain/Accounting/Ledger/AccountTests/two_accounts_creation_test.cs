using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Tests.UnitTests.Domain.Accounting.Ledger.AccountTests
{
    public class two_accounts_creation_test : LedgerTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield return Ledger_Created();
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Ledger_Account_Create("45", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
            yield return Ledger_Account_Create("4534", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Ledger_Account_Added("45", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
            yield return Ledger_Account_Added("4534", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
        }

        [Ignore]
        [Test] 
        public void Test()
        {
            Validate();
        }
    }
}