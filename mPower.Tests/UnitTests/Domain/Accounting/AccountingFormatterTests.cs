using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Accounting
{
    [TestFixture]
    public class AccountingFormatterTests
    {
        [Test]
        public void intuit_balance_converter()
        {
            var intuitBalanceToAggregegatedBalanceInCents = AccountingFormatter.IntuitBalanceToAggregegatedBalanceInCents(-100, AccountLabelEnum.CreditCard);

            Assert.AreEqual(10000, intuitBalanceToAggregegatedBalanceInCents);

        }
    }
}
