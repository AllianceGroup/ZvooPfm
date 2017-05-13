using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Services
{
    public class TransactionLuceneSerivceTest : BaseServiceTest
    {
        private TransactionLuceneService _luceneService;

        public IEnumerable<EntryDocument> GivenEntries()
        {
            yield return new EntryDocument()
            {
                AccountId = "1",
                LedgerId = "1",
                AccountLabel = AccountLabelEnum.Bank,
                AccountName = "1",
                AccountType = AccountTypeEnum.Asset,
                BookedDate = CurrentDate.AddMonths(-12),
                BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                CreditAmountInCents = 100,
                DebitAmountInCents = 0,
                FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(-100, true),
                Imported = true,
                Memo = "Memo",
                OffsetAccountId = "2",
                OffsetAccountName = "2",
                Payee = "Payee",
                TransactionId = _id,
                Id = _id + "e0",
                EntryBalance = 200
            };

            yield return new EntryDocument()
            {
                AccountId = "2",
                LedgerId = "1",
                AccountLabel = AccountLabelEnum.Bank,
                AccountName = "my first account",
                AccountType = AccountTypeEnum.Asset,
                BookedDate = CurrentDate,
                BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                CreditAmountInCents = 0,
                DebitAmountInCents = 100,
                FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(100, true),
                Imported = true,
                Memo = "Memo",
                OffsetAccountId = "20",
                OffsetAccountName = "20",
                Payee = "cool Payee",
                TransactionId = _id,
                Id = _id + "e1",
                EntryBalance = 100
            };

            yield return new EntryDocument()
            {
                AccountId = "1",
                LedgerId = "2",
                AccountLabel = AccountLabelEnum.CreditCard,
                AccountName = "adf",
                AccountType = AccountTypeEnum.Liability,
                BookedDate = CurrentDate.AddMonths(-6),
                BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                CreditAmountInCents = 100,
                DebitAmountInCents = 0,
                FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(-100, true),
                Imported = true,
                Memo = "Memo of most important entry",
                OffsetAccountId = "21",
                OffsetAccountName = "Offset account",
                Payee = "Payee",
                TransactionId = _id,
                Id = _id + "e3",
                EntryBalance = 202
            };
        }

        public override void Setup()
        {
            base.Setup();

            _luceneService = _container.GetInstance<TransactionLuceneService>();

            foreach (var entry in GivenEntries())
            {
                _luceneService.Insert(entry);
            }
        }

        [Test]
        public void service_should_correctly_search_by_date_range()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter()
                                                 {
                                                     BookedDateMinValue = CurrentDate.AddMonths(-1),
                                                     BookedDateMaxValue = CurrentDate.AddMonths(1)
                                                 });

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void service_should_correctly_search_by_entry_amount()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter()
                {
                    MinEntryAmount = 99,
                    MaxEntryAmount = 201
                });

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void service_should_search_by_partial_memo()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter() { SearchText = "important" });

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void service_can_search_by_multiword_payee()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter() { SearchText = "Cool payee" });

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void service_can_search_by_account_name()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter() { SearchText = "first" });

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void service_can_search_by_offset_account_name()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter() { SearchText = "Offset" });

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void service_can_search_by_account_label()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter() { SearchText = "creditcard" });

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void service_can_search_by_account_type()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter() { SearchText = "bank" });

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void service_can_filter_entries_by_labels()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter() { AccountLabels = new List<AccountLabelEnum>() { AccountLabelEnum.Bank, AccountLabelEnum.CreditCard } });

            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void when_service_filter_by_account_id_it_should_also_filter_by_offset_account_id()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter() { AccountId = "2" });

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void service_should_filter_by_ledger_id()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter() { LedgerId = "1" });

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void service_should_filter_by_transaction_id()
        {
            var result =
                _luceneService.SearchByQuery(new EntryLuceneFilter() { TransactionId = _id });

            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void service_should_sort_results_by_booked_date_descending()
        {
            var result = _luceneService.SearchByQuery(new EntryLuceneFilter() { AccountIds = new List<string>() { "1", "2" } });

            int count = result.Count;
            for (int i = 0; i < count; i++)
            {
                if (i + 1 == count)
                    break;
                Assert.LessOrEqual(result[i + 1].BookedDate, result[i].BookedDate);
            }

            Assert.AreEqual(3, result.Count);
        }
    }
}
