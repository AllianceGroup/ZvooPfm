using System;
using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.Domain.Accounting.Ledger.BudgetTests
{
    public class budget_update_test : LedgerTest
    {
        private readonly long _newAmountInCents = (new Random()).Next(5000);

        public override IEnumerable<IEvent> Given()
        {
            yield return Ledger_Created();
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Ledger_Budget_Update("1", CurrentDate.Year, CurrentDate.Month, _newAmountInCents);
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Ledger_Budget_Updated("1", CurrentDate.Year, CurrentDate.Month, _newAmountInCents);
        }

        [Test]
        public void Test()
        {
            Validate();
        }
    }
}