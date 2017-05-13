using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Default.Factories.ViewModels;
using Default.ViewModel.Areas.Shared;
using Moq;
using mPower.Documents.DocumentServices;
using mPower.Documents.Documents.Affiliate;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Mvc;
using mPower.Tests.MoqObjects.DocumentServices;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Web.Factories.ViewModel
{
    [TestFixture]
    public class TransactionsListViewModelFactoryTest : BaseWebTest
    {
        private LedgerDocumentServiceMock _ledgerDocumentServiceMock;
        private TransactionsListViewModelFactory _factory;
        private RequestContext _requestContext;
        private Mock<AffiliateDocumentService> _affiliateDocumentServiceMock;

        [SetUp]
        public void TestSetup()
        {
            _ledgerDocumentServiceMock = MockFactory
                .Create<LedgerDocumentServiceMock>()
                .AddGetById();
            _affiliateDocumentServiceMock = new Mock<AffiliateDocumentService>(_container.GetInstance<MongoRead>());
            _affiliateDocumentServiceMock.Setup(x => x.GetById(It.IsAny<string>())).Returns(new AffiliateDocument());
            _affiliateDocumentServiceMock.Setup(x => x.GetAll()).Returns(new List<AffiliateDocument>());

            var mock = new Mock<IObjectRepository>();
            mock.Setup(x => x.Load<TransactionClientFilter, List<Entry>>(It.IsAny<TransactionClientFilter>()))
                .Returns(new List<Entry>());

            _factory = new TransactionsListViewModelFactory(mock.Object, _ledgerDocumentServiceMock.Object, _affiliateDocumentServiceMock.Object);
            _requestContext = new RequestContext()
            {
                RouteData = new RouteData(),
                HttpContext = new Mock<HttpContextBase>().Object
            };
        }

        [Test]
        public void when_filtering_by_account_id_mode_should_be_null()
        {


            var filter = new TransactionClientFilter(_ledgerDocumentServiceMock.FakeLedger.Id, null, _requestContext)
            {
                accountId = _ledgerDocumentServiceMock.FakeLedger.Accounts.First().Id,
                mode = "bank"
            };
            var model = _factory.Load(filter);

            Assert.AreEqual(null, filter.mode);
        }


        [Test]
        public void properly_map_client_filter_data()
        {
            var filter = new TransactionClientFilter(_ledgerDocumentServiceMock.FakeLedger.Id, null, _requestContext)
            {
                mode = "bank",
                request = "search key",
                from = "03/03/2011",
                to = "03/03/2012"
            };

            var model = _factory.Load(filter);
            Assert.True(model.HiddenAuxDataForScript.ContainsKey("filterRequest"));
            Assert.True(model.HiddenAuxDataForScript.ContainsKey("searchRequest"));
            Assert.True(model.HiddenAuxDataForScript.ContainsKey("totalEntryCount"));
            Assert.True(model.HiddenAuxDataForScript.ContainsKey("s"));
            Assert.True(model.HiddenAuxDataForScript.ContainsKey("type"));

            Assert.AreEqual(filter.request, model.SearchText);
            Assert.AreEqual(AccountLabelEnum.Bank, model.Mode);
            Assert.AreEqual(filter.from, model.From);
            Assert.AreEqual(filter.to, model.To);
        }
    }
}
