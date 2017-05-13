using System;
using System.Diagnostics;
using System.Linq;
using Default.Factories.ViewModels;
using Default.Models;
using Default.ViewModel.Areas.Finance.TrendsController;
using Moq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;
using mPower.Tests.MoqObjects.DocumentServices;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Web.Factories.ViewModel
{
    [TestFixture]
    public class TrendsViewModelFactoryTest : BaseWebTest
    {
        private LedgerDocumentService _ledgerDocumentService;
        private TransactionsStatisticDocumentService _statisticService;
        private TrendsViewModelFactory _factory;

        private AccountDocument _accountDocument;
        private TrendsViewModelFilter _accountFilter;
        private TrendsViewModelFilter _fullFilter;

        [SetUp]
        public void TestSetup()
        {
            var ledgerMock = MockFactory.Create<LedgerDocumentServiceMock>()
                                        .AddFindOne()
                                        .AddGetById();
            _ledgerDocumentService = ledgerMock.Object;

            _statisticService = MockFactory.Create<TransactionsStatisticDocumentServiceMock>().Object;

            _accountDocument = ledgerMock.FakeLedger.Accounts.FirstOrDefault(x => x.TypeEnum == AccountTypeEnum.Expense);
            
            _factory = new TrendsViewModelFactory(_ledgerDocumentService, _statisticService);
        }

        [Test]
        public void TrendsCorrectlyFilteringByMonth()
        {
            var filter = GetFilter();
            var year = DateTime.Now.Year;
            filter.Year = year;
            var month = DateTime.Now.Month - 1;
            filter.Month = month;
            var model = _factory.Load(filter);
            Assert.AreEqual(year, model.Date.Year);
            Assert.AreEqual(month, model.Date.Month);
        }

        [Test]
        public void TrendsCorrectlyFilteringByAllTime()
        {
            var filter = GetFilter();
            filter.Filter = TrendModelFilterEnum.AllTime;
            var model = _factory.Load(filter);
            Assert.AreEqual(TrendModelFilterEnum.AllTime, model.SelectedFilter);
        }

        [Test]
        public void TrendsCorrectlyFilteringByCategory()
        {
            var filter = AccountFilter;
            var account = _accountDocument;
            var model = _factory.Load(filter);
            Assert.AreEqual(account.Id, model.SelectedCategoryId);
            Assert.AreEqual(account.Name, model.SelectedCategoryName);
        }

        [Test]
        public void AllPropertiesAreMapped()
        {
            var filter = GetFullFilter();
            var model = _factory.Load(filter);

            Assert.AreEqual(filter.All, model.All);
            Assert.AreEqual(filter.ShowFormat, filter.ShowFormat);
            Assert.AreEqual(filter.AccountId , model.SelectedCategoryId);

            Assert.IsNotEmpty(model.Monthes);
            Assert.IsNotEmpty(model.TrendFilters);
            Assert.IsNotEmpty(model.ShowFormats);
        }

        [Test]
        public void AccountNameIsCorrect()
        {
            var filter = AccountFilter;
            var account = _accountDocument;
            var model = _factory.Load(filter);
            Assert.AreEqual(account.Name, model.SelectedCategoryName);
        }

        [Test]
        public void TimeFilterWorksCorrectly()
        {
            var filter = GetFullFilter();

            filter.Filter = TrendModelFilterEnum.AllTime;
            var model = _factory.Load(filter);
            Assert.AreEqual(TrendModelFilterEnum.AllTime, model.SelectedFilter);

            filter.Filter = TrendModelFilterEnum.PrevMonth;
            var date = DateTime.Now.AddMonths(-1);
            filter.Month = date.Month;
            filter.Year = date.Year;
            model = _factory.Load(filter);
            Assert.AreEqual(TrendModelFilterEnum.PrevMonth, model.SelectedFilter);
            Assert.AreEqual(date.Month,model.Date.Month);
            Assert.AreEqual(date.Year, model.Date.Year);

            filter.Filter = TrendModelFilterEnum.SelectedMonth;
            filter.Month = 1;
            filter.Year = DateTime.Now.Year;
            model = _factory.Load(filter);
            Assert.AreEqual(TrendModelFilterEnum.SelectedMonth, model.SelectedFilter);
            Assert.AreEqual(1, model.Date.Month);
            Assert.AreEqual(DateTime.Now.Year, model.Date.Year);

            filter.Filter = TrendModelFilterEnum.Year;
            filter.Year = DateTime.Now.Year;
            model = _factory.Load(filter);
            Assert.AreEqual(TrendModelFilterEnum.Year, model.SelectedFilter);
        }

        [Test]
        public void CategoriesListContainsParentAccount()
        {
            var filter = AccountFilter;
            var account = _accountDocument;
            var model = _factory.Load(filter);
            Assert.IsTrue(model.Categories.Any(x => x.AccountId == account.Id));
        }

        private TrendsViewModelFilter GetFilter()
        {
            var filter = new TrendsViewModelFilter();
            filter.LedgerId = _ledgerDocumentService.FindOne().Id;
            filter.FilterByAccountUrl = String.Empty;
            return filter;
        }

        private TrendsViewModelFilter AccountFilter
        {
            get
            {
                if (_accountFilter != null)
                {
                    return _accountFilter;
                }
                var filter = GetFilter();
                filter.AccountId = _accountDocument.Id;
                filter.Filter = TrendModelFilterEnum.AllTime;
                return _accountFilter = filter;
            }
        }

        private TrendsViewModelFilter GetFullFilter()
        {
            if (_fullFilter != null)
            {
                return _fullFilter;
            }
            var filter = new TrendsViewModelFilter()
                             {
                                 All = true,
                                 Filter = TrendModelFilterEnum.AllTime,
                                 FilterByAccountUrl = "",
                                 LedgerId = "1",
                                 Month = 0,
                                 Year = 0,
                                 ShowFormat = ShowFormatEnum.Spending,
                                 TakeCategories = 0
                             };
            return _fullFilter = filter;
        }
    }
}