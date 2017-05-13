using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using Default.Factories.ViewModels;
using Default.ViewModel.Areas.Business.AccountsController;
using Default.ViewModel.Areas.Business.BusinessController;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Framework.Mvc;
using mPower.Tests.MoqObjects.DocumentServices;

namespace mPower.Tests.UnitTests.Web.Factories.ViewModel
{
    [TestFixture]
    public class AccountViewModelFactoryTest : BaseWebTest
    {
        private Mock<IObjectRepository> _objectRepository;
        private LedgerDocumentServiceMock _ledgerService;
        private AccountListViewModelFactory _accountListViewModelFactory;

        protected AccountViewModelFactory Factory { get; set; }

        [SetUp]
        public void TestSetup()
        {
            _ledgerService = MockFactory.Create<LedgerDocumentServiceMock>().AddGetById();
            _accountListViewModelFactory = new AccountListViewModelFactory(_ledgerService.Object);
            _objectRepository = new Mock<IObjectRepository>();
            _objectRepository
                .Setup(x => x.Load<string, IEnumerable<Account>>(It.IsAny<string>()))
                .Returns((string id) => _accountListViewModelFactory.Load(id));
            Factory = new AccountViewModelFactory(_objectRepository.Object, _ledgerService.Object);
            _objectRepository
                .Setup(x => x.Load<string, ChartOfAccountsViewModel>(It.IsAny<string>()))
                .Returns((string id) => Factory.Load(id));
        }

        [Test]
        public void ordering_works_correctly()
        {
            var filter = new AccountsSortingFilter
                             {
                                 LedgerId = _ledgerService.FakeLedger.Id,
                                 Field = AccountSortFieldEnum.Balance,
                                 Direction = SortDirection.Ascending
                             };
            //Ascending - Balance
            var model = Factory.Load(filter);
            for (int i = 0; i < model.Accounts.Count - 1; i++)
            {
                Assert.GreaterOrEqual(model.Accounts[i + 1].Balance, model.Accounts[i].Balance);
            }

            //Descending - Name
            filter.Direction = SortDirection.Descending;
            filter.Field = AccountSortFieldEnum.Name;
            model = Factory.Load(filter);
            for (int i = 0; i < model.Accounts.Count - 1; i++)
            {
                Assert.GreaterOrEqual(model.Accounts[i].Name, model.Accounts[i + 1].Name);
            }

            //Ascending - Name
            filter.Direction = SortDirection.Ascending;
            filter.Field = AccountSortFieldEnum.Type;
            model = Factory.Load(filter);
            for (int i = 0; i < model.Accounts.Count - 1; i++)
            {
                Assert.GreaterOrEqual(model.Accounts[i + 1].Type, model.Accounts[i].Type);
            }
        }

        [Test]
        public void null_account_id_argument_throws_exception()
        {

            Assert.Throws<Exception>(() =>
                              {
                                  var model = Factory.Load(new EditAccountFilter()
                                                               {
                                                                   AccountId = null,
                                                                   LedgerId = _ledgerService.FakeLedger.Id
                                                               });
                              });
        }

        [Test]
        public void all_needed_properties_is_mapped()
        {
            var account = _ledgerService.FakeLedger.Accounts.First();
            var model = Factory.Load(new EditAccountFilter()
            {
                AccountId = account.Id,
                LedgerId = _ledgerService.FakeLedger.Id
            });
            Assert.AreEqual(account.Id,model.Id);
            Assert.AreEqual(account.LabelEnum,model.Label);
            Assert.AreEqual(account.Name,model.Name);
            Assert.AreEqual(account.Number,model.Number);
            Assert.AreEqual(account.TypeEnum,model.Type);
            Assert.AreEqual(account.ParentAccountId,model.ParentAccountId);
            Assert.AreEqual(AccountingFormatter.CentsToDollars(account.CreditLimitInCents),model.CreditLimitInDollars);
            Assert.AreEqual(AccountingFormatter.CentsToDollars(account.MinMonthPaymentInCents),model.MinMonthPaymentInDollars);
            Assert.AreEqual(account.InterestRatePerc,model.InterestRatePercentage);
            Assert.AreEqual(account.Description,model.Description);
        }
    }
}