using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Services
{
    [TestFixture]
    public class TransactionDocumentServiceTests : BaseServiceTest
    {
        private TransactionDocumentService _service;

        public override void Setup()
        {
            base.Setup();
            _service = _container.GetInstance<TransactionDocumentService>();
        }

        public override System.Collections.Generic.IEnumerable<object> Given()
        {
            yield return new TransactionDocument
            {
                Id = "1",
                LedgerId = "1",
                Entries = new List<TransactionEntryDocument>
                {
                    new TransactionEntryDocument {AccountId = "1"},
                    new TransactionEntryDocument {AccountId = "2"},
                },
                BookedDate = CurrentDate.AddDays(-5),
            };
            yield return new TransactionDocument
            {
                Id = "2",
                LedgerId = "1",
                Entries = new List<TransactionEntryDocument>
                {
                    new TransactionEntryDocument {AccountId = "1"},
                    new TransactionEntryDocument {AccountId = "3"},
                },
                BookedDate = CurrentDate.AddDays(-3),
            };
            yield return new TransactionDocument
            {
                Id = "3",
                LedgerId = "1",
                Entries = new List<TransactionEntryDocument>
                {
                    new TransactionEntryDocument {AccountId = "2"},
                    new TransactionEntryDocument {AccountId = "3"},
                },
                BookedDate = CurrentDate.AddDays(-1),
            };
            yield return new TransactionDocument
            {
                Id = "4",
                LedgerId = "2",
                Entries = new List<TransactionEntryDocument>
                {
                    new TransactionEntryDocument {AccountId = "1"},
                    new TransactionEntryDocument {AccountId = "4"},
                },
                BookedDate = CurrentDate,
            };
        }

        [Test]
        public void ordering_works_correctly()
        {
            var transactions = _service.GetByFilter(new TransactionFilter {LedgerId  = "1", SortByFiled = TransactionsSortFieldEnum.BookedDate});
            Assert.Greater(transactions.Count, 1);
            for (var i = 0; i < transactions.Count - 1; i++)
            {
                Assert.LessOrEqual(transactions[i].BookedDate, transactions[i + 1].BookedDate);
            }

            transactions = _service.GetByFilter(new TransactionFilter {LedgerId = "1", SortByFiled = TransactionsSortFieldEnum.BookedDateDescending});
            Assert.Greater(transactions.Count, 1);
            for (var i = 0; i < transactions.Count - 1; i++)
            {
                Assert.GreaterOrEqual(transactions[i].BookedDate, transactions[i + 1].BookedDate);
            }
        }

        [Test]
        public void test_filtering_by_ledgerId()
        {
            const string ledgerId = "1";
            var transactions = _service.GetByFilter(new TransactionFilter {LedgerId = ledgerId});
            Assert.AreEqual(3, transactions.Count);
            foreach (var trans in transactions)
            {
                Assert.AreEqual(ledgerId, trans.LedgerId);
            }
        }

        [Test]
        public void test_filtering_by_accountId()
        {
            const string accountId = "1";
            var transactions = _service.GetByFilter(new TransactionFilter {AccountId = accountId});
            Assert.AreEqual(3, transactions.Count);
            foreach (var trans in transactions)
            {
                Assert.IsTrue(trans.Entries.Any(x => x.AccountId == accountId));
            }
        }

        [Test]
        public void test_filtering_by_accountsIds()
        {
            var accountsIds = new List<string> {"2", "4"};
            var transactions = _service.GetByFilter(new TransactionFilter {AccountIds = accountsIds});
            Assert.AreEqual(3, transactions.Count);
            foreach (var trans in transactions)
            {
                Assert.IsTrue(trans.Entries.Any(x => accountsIds.Contains(x.AccountId)));
            }
        }

        [Test]
        public void test_filtering_by_entry_from_date()
        {
            var fromDate = CurrentDate.AddDays(-1);
            var transactions = _service.GetByFilter(new TransactionFilter {EntryFromDate = fromDate});
            Assert.AreEqual(2, transactions.Count);
            foreach (var trans in transactions)
            {
                Assert.GreaterOrEqual(trans.BookedDate, fromDate.Date);
            }
        }

        [Test]
        public void test_filtering_by_entry_to_date()
        {
            var toDate = CurrentDate.AddDays(-1);
            var transactions = _service.GetByFilter(new TransactionFilter {EntryToDate = toDate});
            Assert.AreEqual(3, transactions.Count);
            foreach (var trans in transactions)
            {
                Assert.LessOrEqual(trans.BookedDate.Date, toDate);
            }
        }

        [Test]
        public void test_filtering_by_entry_between_dates()
        {
            var fromDate = CurrentDate.AddDays(-3);
            var toDate = CurrentDate.AddDays(-1);
            var transactions = _service.GetByFilter(new TransactionFilter {EntryFromDate = fromDate, EntryToDate = toDate});
            Assert.AreEqual(2, transactions.Count);
            foreach (var trans in transactions)
            {
                Assert.GreaterOrEqual(trans.BookedDate, fromDate.Date);
                Assert.LessOrEqual(trans.BookedDate.Date, toDate);
            }
        }
    }
}