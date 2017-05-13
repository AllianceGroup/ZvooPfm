using NUnit.Framework;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;

namespace mPower.Tests.UnitTests.Domain.Accounting
{
    [TestFixture]
   
    public class TransactionDocumentEventHandlerTest
    {

        [Test]
        public void credit_asset_10_returns_negative_1()
        {
            var result = AccountingFormatter.ConvertToDollarsThenFormat(0, 100, AccountTypeEnum.Asset);

            Assert.AreEqual("($1.00)",result);
        }

        [Test]
        public void debit_asset_10_returns_positive_1()
        {
            var result = AccountingFormatter.ConvertToDollarsThenFormat(100, 0, AccountTypeEnum.Asset);

            Assert.AreEqual("$1.00", result);
        }

        [Test]
        public void credit_liability_10_returns_positive_1()
        {
            var result = AccountingFormatter.ConvertToDollarsThenFormat(0, 100, AccountTypeEnum.Liability);

            Assert.AreEqual("$1.00", result);
        }

        [Test]
        public void debit_liability_10_returns_negative_1()
        {
            var result = AccountingFormatter.ConvertToDollarsThenFormat(100, 0, AccountTypeEnum.Liability);

            Assert.AreEqual("($1.00)", result);
        }


        [Test]
        public void credit_equity_10_returns_positive_1()
        {
            var result = AccountingFormatter.ConvertToDollarsThenFormat(0, 100, AccountTypeEnum.Equity);

            Assert.AreEqual("$1.00", result);
        }

        [Test]
        public void debit_equity_10_returns_negative_1()
        {
            var result = AccountingFormatter.ConvertToDollarsThenFormat(100, 0, AccountTypeEnum.Equity);

            Assert.AreEqual("($1.00)", result);
        }

        [Test]
        public void credit_income_10_returns_positive_1()
        {
            var result = AccountingFormatter.ConvertToDollarsThenFormat(0, 100, AccountTypeEnum.Income);

            Assert.AreEqual("$1.00", result);
        }

        [Test]
        public void debit_income_10_returns_negative_1()
        {
            var result = AccountingFormatter.ConvertToDollarsThenFormat(100, 0, AccountTypeEnum.Income);

            Assert.AreEqual("($1.00)", result);
        }

        [Test]
        public void credit_expense_10_returns_negative_1()
        {
            var result = AccountingFormatter.ConvertToDollarsThenFormat(0, 100, AccountTypeEnum.Expense);

            Assert.AreEqual("($1.00)", result);
        }

        [Test]
        public void debit_expense_10_returns_positive_1()
        {
            var result = AccountingFormatter.ConvertToDollarsThenFormat(100, 0, AccountTypeEnum.Expense);

            Assert.AreEqual("$1.00", result);
        }
    }
}
