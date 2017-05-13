using System.Collections.Generic;
using System.Linq;
using Default.Factories.ViewModels;
using NUnit.Framework;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Tests.MoqObjects.DocumentServices;
using mPower.Tests.MoqObjects.Lucene;
using System;
using mPower.Framework.Utils;

namespace mPower.Tests.UnitTests.Web.Factories.ViewModel
{
    [TestFixture]
    public class LegerViewModelFactoryTest : BaseWebTest
    {
        private LegerViewModelFactory _factory;
        private LedgerDocumentServiceMock _ledgerDocumentServiceMock;
        private BudgetDocumentServiceMock _budgetDocumentServiceMock;
        private TransactionLuceneSerivceMock _transactionLuceneSerivceMock;

        [SetUp]
        public void TestSetup()
        {
            _transactionLuceneSerivceMock =
                MockFactory.Create<TransactionLuceneSerivceMock>()
                .AddSearchByQueryResults();
            _ledgerDocumentServiceMock = MockFactory
                .Create<LedgerDocumentServiceMock>()
                .AddGetById();
                
            _budgetDocumentServiceMock = MockFactory
                .Create<BudgetDocumentServiceMock>()
                .AddGetByFilterWithSubBudgets();

            var entryDocumentServiceMock = MockFactory.Create<EntryDocumentServiceMock>().AddGetByFilter();
            var affiliateDocumentServiceMock = MockFactory.Create<AffiliateDocumentServiceMock>().AddGetById();

            _factory = new LegerViewModelFactory(affiliateDocumentServiceMock.Object, _ledgerDocumentServiceMock.Object,
                entryDocumentServiceMock.Object, _budgetDocumentServiceMock.Object, _transactionLuceneSerivceMock.Object,
                _container.GetInstance<UploadUtil>(), _container.GetInstance<CommandService>());
        }

        [Test]
        public void factory_corecctly_load_map_categories_for_specified_ledger()
        {
            var accounts = _ledgerDocumentServiceMock.FakeLedger.Accounts;

            var categories = _factory.Load(_ledgerDocumentServiceMock.FakeLedger.Id);
            foreach (var category in categories)
            {
                var account = accounts.Single(x => x.Id == category.AccountId);

                Assert.AreEqual(category.AccountId, account.Id);
                Assert.AreEqual(category.AccountLabel, account.LabelEnum);
                Assert.AreEqual(category.AccountName, account.Name);
                Assert.AreEqual(category.AccountType, account.TypeEnum);
                Assert.AreEqual(category.ParentAccountId, account.ParentAccountId);
            }
        }

        [Test]
        public void when_just_account_id_come_from_client_method_return_account_id_to_lucene()
        {
            var clientFilter = new TransactionClientFilter("1", null, null) { accountId = "44" };
            var luceneFilter = _factory.BuildLuceneSearchFilter(clientFilter);

            Assert.AreEqual(luceneFilter.AccountId, clientFilter.accountId);
        }

        [Test]
        public void when_account_id_and_include_subaccounts_then_add_all_parent_and_subaccounts_ids()
        {
            var accountId = "5";

            var clientFilter = new TransactionClientFilter("1", null, null) { accountId = accountId, s = 1 };
            var luceneFilter = _factory.BuildLuceneSearchFilter(clientFilter);
            var shouldReturn = new List<string>() { accountId };
            shouldReturn.AddRange(_ledgerDocumentServiceMock.FakeLedger.Accounts.Where(x => x.ParentAccountId == accountId).Select(x => x.Id).ToList());

            foreach (var id in shouldReturn)
            {
                Assert.True(luceneFilter.AccountIds.Contains(id));
            }

            Assert.AreEqual(luceneFilter.AccountId, null);
        }

        [Test]
        public void when_search_key_is_money_then_filter_should_return_min_max()
        {
            decimal amount = 2.15m;

            var clientFilter = new TransactionClientFilter("1", null, null) { request = amount.ToString() };
            var luceneFilter = _factory.BuildLuceneSearchFilter(clientFilter);
            Assert.AreEqual(172, luceneFilter.MinEntryAmount);
            Assert.AreEqual(258, luceneFilter.MaxEntryAmount);
        }

        [Test]
        public void when_search_key_is_text_then_filter_should_return_search_key()
        {
            string searchKey = "hello";

            var clientFilter = new TransactionClientFilter("1", null, null) { request = searchKey };
            var luceneFilter = _factory.BuildLuceneSearchFilter(clientFilter);
            Assert.AreEqual(searchKey, luceneFilter.SearchText);
        }

        [Test]
        public void when_search_mode_all_labels_should_be_empty()
        {
            string mode = "all";
            var clientFilter = new TransactionClientFilter("1", null, null) { mode = mode };
            var luceneFilter = _factory.BuildLuceneSearchFilter(clientFilter);

            Assert.AreEqual(0, luceneFilter.AccountLabels.Count);
        }

        [Test]
        public void when_search_mode_bank_labels_should_contains_bank()
        {
            string mode = "bank";
            var clientFilter = new TransactionClientFilter("1", null, null) { mode = mode };
            var luceneFilter = _factory.BuildLuceneSearchFilter(clientFilter);

            Assert.AreEqual(1, luceneFilter.AccountLabels.Count);
            Assert.AreEqual(AccountLabelEnum.Bank, luceneFilter.AccountLabels[0]);
        }

        [Test]
        public void when_user_specified_date_range_filter_should_return_dates()
        {
            string from = "03/20/2009";
            string to = "03/20/2010";
            var clientFilter = new TransactionClientFilter("1", null, null) { from = from, to = to };
            var luceneFilter = _factory.BuildLuceneSearchFilter(clientFilter);

            Assert.AreEqual(DateTime.Parse(from), luceneFilter.BookedDateMinValue);
            Assert.AreEqual(DateUtil.GetEndOfDay(DateTime.Parse(to)), luceneFilter.BookedDateMaxValue);
        }

        [Test]
        public void when_type_and_dateRange_load_budgets_and_take_ids_of_budgets_accounts_and_subbudgets_accounts()
        {
            var budgets = _budgetDocumentServiceMock.BudgetsWithSubBudgets.Where(x => x.AccountType == AccountTypeEnum.Expense);
            string from = "03/20/2009";
            string to = "03/20/2010";

            var clientFilter = new TransactionClientFilter("1", null, null) { type = "5", from = from, to = to };
            var luceneFilter = _factory.BuildLuceneSearchFilter(clientFilter);

            var expectedIds = new List<string>();
            var parentAccountIds = budgets.Select(x => x.AccountId);
            var subIds = _ledgerDocumentServiceMock.FakeLedger.Accounts.Where(x => parentAccountIds.Contains(x.ParentAccountId)).Select(x => x.Id);
            expectedIds.AddRange(parentAccountIds);
            expectedIds.AddRange(subIds);

            foreach (var id in expectedIds)
            {
                Assert.True(luceneFilter.AccountIds.Contains(id));
            }
        }

        [Test]
        public void when_user_search_then_factory_correctly_map_results_from_lucene()
        {
            var luceneEntries = _transactionLuceneSerivceMock.FakeLuceneEntries;

            var searchResults = _factory.Load(new TransactionClientFilter("1", null, null) {affiliateId = "1"});

            foreach (var res in searchResults)
            {
                var entry = luceneEntries.Single(x => x.AccountId == res.AccountId);

                Assert.AreEqual(entry.AccountId, res.AccountId);
                Assert.AreEqual(entry.OffsetAccountName, res.OffsetAccountName);
                Assert.AreEqual(entry.OffsetAccountId, res.OffsetAccountId);
                Assert.AreEqual(entry.FormattedAmountInDollars, res.FormattedAmountInDollars);
                Assert.AreEqual(entry.Payee, res.Payee);
                Assert.AreEqual(entry.BookedDate.Date, res.BookedDate.Date);
                Assert.AreEqual(entry.TransactionId, res.TransactionId);
                Assert.AreEqual(entry.Memo, res.Memo);
                Assert.True(entry.AccountName.Length <= 25);
            }
        }
    }
}
