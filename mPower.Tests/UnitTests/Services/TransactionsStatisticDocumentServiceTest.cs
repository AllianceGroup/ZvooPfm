using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;

namespace mPower.Tests.UnitTests.Services
{
    [TestFixture]
    public class TransactionsStatisticDocumentServiceTest : BaseServiceTest
    {
        private TransactionsStatisticDocumentService _service;

        private readonly DateTime date = new DateTime(2012, 1, 1);
        private List<TransactionsStatisticDocument> _docs = new List<TransactionsStatisticDocument>();

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _service = _container.GetInstance<TransactionsStatisticDocumentService>();
        }

        public override IEnumerable<object> Given()
        {
            yield return CreateStatisticDocument();
            yield return CreateStatisticDocument(month: date.Month -1);
            yield return CreateStatisticDocument(month: date.Month -2);
            yield return CreateStatisticDocument(month: date.Month -3);
            yield return CreateStatisticDocument(month: date.Month -4);
            yield return CreateStatisticDocument(month: date.Month -5);
            yield return CreateStatisticDocument("2", AccountTypeEnum.Equity);
            yield return CreateStatisticDocument("3", AccountTypeEnum.Expense);
            yield return CreateStatisticDocument("4", AccountTypeEnum.Income);
            yield return CreateStatisticDocument("5", AccountTypeEnum.Liability);
        }


        private TransactionsStatisticDocument CreateStatisticDocument
            (string accountId = "1", 
            AccountTypeEnum accountType = AccountTypeEnum.Asset,
            long creditAmountInCents = 5000,
            long debitAmountInCents = 10000,
            string ledgerId = "1",
            int? month = null,
            int? year = null)
        {

            var id = Guid.NewGuid().ToString();
            var doc = new TransactionsStatisticDocument
                       {
                           Id = id ,
                           AccountId = accountId,
                           AccountType = accountType,
                           CreditAmountInCents = creditAmountInCents,
                           DebitAmountInCents = debitAmountInCents,
                           LedgerId = ledgerId,
                           Month = month ?? date.Month,
                           Year = year ?? date.Year
                       };
            _docs.Add(doc);
            return doc;
        }

        [Test]
        public void add_statistic_with_valid_input_increase_debt_amount()
        {
            _service.AddStatistic("1", "1", AccountTypeEnum.Asset, date, 100, 0);
            var stat = GetDocument("1");
            Assert.AreEqual(10100,stat.DebitAmountInCents);
        }

        [Test]
        public void add_statistic_with_valid_input_decrease_debt_amount()
        {
            _service.AddStatistic("1", "2", AccountTypeEnum.Equity, date, -100, 0);
            var stat = GetDocument("2");
            Assert.AreEqual(9900, stat.DebitAmountInCents);
        }

        [Test]
        public void add_statistic_with_valid_input_increase_credit_amount()
        {
            _service.AddStatistic("1", "3", AccountTypeEnum.Expense, date, 0, 100);
            var stat = GetDocument("3");
            Assert.AreEqual(5100, stat.CreditAmountInCents);
        }

        [Test]
        public void add_statistic_with_valid_input_decrease_credit_amount()
        {
            _service.AddStatistic("1", "4", AccountTypeEnum.Income, date, 0, -100);
            var stat = GetDocument("4");
            Assert.AreEqual(4900, stat.CreditAmountInCents);
        }

        [Test]
        public void add_statistic_with_invalid_input_does_not_affect_amounts()
        {
            _service.AddStatistic("1", "5", AccountTypeEnum.Income, DateTime.Now.AddYears(1), 1111,1111);
            var stat = GetDocument("5");
            Assert.AreEqual(5000,stat.CreditAmountInCents);
            Assert.AreEqual(10000,stat.DebitAmountInCents);
        }

        [Test]
        public void GetLastSixMonthsAvr_calculates_correclty()
        {
            var avr = _service.GetLastSixMonthsAvr("1", "1", date);
            Assert.AreEqual(5000,avr);
        }

        [Test]
        public void GetLastSixMonthsAvr_calculates_correclty_if_date_min_value()
        {
            var avr = _service.GetLastSixMonthsAvr("1", "1", DateTime.MinValue);
            Assert.AreEqual(0,avr);
        }

        [Test]
        public void GetProfitLostStatistic_result_is_in_correct_date_range()
        {
            var result = _service.GetProfitLostStatistic("1", date);
            foreach (var statisticDocument in result)
            {
                Assert.AreEqual(date.Year,statisticDocument.Year);
                Assert.AreEqual(date.Month,statisticDocument.Month);
            }
        }

        private TransactionsStatisticDocument GetDocument(string accountId, string ledgerId = "1")
        {
            var query = Query.And(Query.EQ("AccountId", accountId), Query.EQ("LedgerId", ledgerId));
            return _service.GetByQuery(query).FirstOrDefault();
        }
    }
}