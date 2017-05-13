using System.Collections.Generic;
using System.Linq;
using Default.Factories.ViewModels;
using Default.ViewModel.Areas.Business.BusinessController;
using NUnit.Framework;
using mPower.Tests.MoqObjects.DocumentServices;

namespace mPower.Tests.UnitTests.Web.Factories.ViewModel
{
    [TestFixture]
    public class AccountListViewModelFactoryTest : BaseWebTest
    {
        private LedgerDocumentServiceMock _ledgerService;

        protected AccountListViewModelFactory Factory { get; set; }

        [SetUp]
        public void TestSetup()
        {
            _ledgerService = MockFactory.Create<LedgerDocumentServiceMock>().AddGetById();
            Factory = new AccountListViewModelFactory(_ledgerService.Object);
        }

        [Test]
        public void all_not_archived_accounts_are_present()
        {
            var expected = _ledgerService.Object.GetById(_ledgerService.FakeLedger.Id).Accounts.Where(x => !x.Archived).Count();
            var  result = Factory.Load(_ledgerService.FakeLedger.Id);
            var actual = CalculdateNodes(result);
            Assert.AreEqual(expected,actual);
        }

        private int CalculdateNodes(IEnumerable<Account> items)
        {
            var count = 0;
            foreach (var account in items)
            {
                count++;
                if (account.Children != null && account.Children.Count() != 0)
                {
                    count += CalculdateNodes(account.Children);
                }
            }
            return count;
        }
    }
}