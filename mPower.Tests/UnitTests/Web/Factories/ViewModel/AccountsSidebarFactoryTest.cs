using System.Linq;
using Default.Factories.ViewModels;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc;
using mPower.Tests.MoqObjects.Common;
using mPower.Tests.MoqObjects.DocumentServices;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Web.Factories.ViewModel
{
    public class AccountsSidebarFactoryTest : BaseWebTest
    {
        private AccountsSidebarFactory _factory;
        private CommandServiceMock _commandServiceMock;
        private LedgerDocumentServiceMock _ledgerDocumentServiceMock;


        [SetUp]
        public void TestSetup()
        {
            var contentServiceItemDocumentServiceMock =
                 MockFactory.Create<ContentServiceItemDocumentServiceMock>()
                 .AddGetByIdReturnNull();

            _commandServiceMock = MockFactory.Create<CommandServiceMock>().SaveCommandsInAMock();

            _ledgerDocumentServiceMock = MockFactory
                .Create<LedgerDocumentServiceMock>()
                .AddGetById();

            var objectRepository = _container.GetInstance<IObjectRepository>();

            _factory = new AccountsSidebarFactory(_ledgerDocumentServiceMock.Object,
                                                  objectRepository);
        }

        [Test]
        public void check_that_account_sidebar_not_throw_errors()
        {
            var model = _factory.Load(_ledgerDocumentServiceMock.FakeLedger.Id);

            Assert.NotNull(model);
        }

        [Test]
        public void account_sidebar_should_not_contains_uncknown_cash()
        {
            var model = _factory.Load(_ledgerDocumentServiceMock.FakeLedger.Id);
            var uncknownCash = model.Accounts.FirstOrDefault(x => x.Name == "Unknown Cash");

            Assert.Null(uncknownCash);
        }

        [Test]
        public void account_sidebat_should_include_all_banks_accounts_except_uc()
        {
            var model = _factory.Load(_ledgerDocumentServiceMock.FakeLedger.Id);
            var banksAccountCount = _ledgerDocumentServiceMock.FakeLedger.Accounts.Count(x => x.LabelEnum == AccountLabelEnum.Bank && x.Name != "Unknown Cash");

            Assert.AreEqual(banksAccountCount, model.Accounts.Count);
        }

        [Test]
        public void account_sidebat_should_include_all_loans_accounts()
        {
            var model = _factory.Load(_ledgerDocumentServiceMock.FakeLedger.Id);
            var expected = _ledgerDocumentServiceMock.FakeLedger.Accounts.Count(x => x.LabelEnum == AccountLabelEnum.Loan);

            Assert.AreEqual(expected, model.Loans.Count);
        }

        [Test]
        public void account_sidebat_should_include_all_creditCard_accounts()
        {
            var model = _factory.Load(_ledgerDocumentServiceMock.FakeLedger.Id);
            var expected = _ledgerDocumentServiceMock.FakeLedger.Accounts.Count(x => x.LabelEnum == AccountLabelEnum.CreditCard);

            Assert.AreEqual(expected, model.CreditCards.Count);
        }

        [Test]
        public void account_sidebat_should_include_all_investments_accounts()
        {
            var model = _factory.Load(_ledgerDocumentServiceMock.FakeLedger.Id);
            var expected = _ledgerDocumentServiceMock.FakeLedger.Accounts.Count(x => x.LabelEnum == AccountLabelEnum.Investment);

            Assert.AreEqual(expected, model.Investments.Count);
        }
    }
}
