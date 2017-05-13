using System.Collections.Generic;
using Default.ViewModel.Areas.Business.BusinessController;
using Default.ViewModel.Areas.Finance.BudgetController.Filters;
using Default.ViewModel.Areas.Shared;
using NUnit.Framework;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc;
using mPower.Framework.Services;
using mPower.Tests.MoqObjects;

namespace mPower.Tests.UnitTests.Services
{
    [TestFixture]
    public class ViewModelBuilderTests
    {
        private IObjectRepository _objectRepository;


        [TestFixtureSetUp]
        public void FixtureSetup()
        {

        }


        [Test]
        [Ignore]
        public void createBusinessAccount_maps_properly()
        {
            var accountDoc = new AccountDocument
            {
                Id = "1",
                Denormalized = new AccountDocument.DenormalizedData {Balance = 1},
                Name = "Name",
                Number = "1",
                TypeEnum = AccountTypeEnum.Asset,
                LabelEnum = AccountLabelEnum.Bank,
                IntuitInstitutionId = 1,
                                     
            };

            var biz = _objectRepository.Load<AccountDocument, BusinessAccount>(accountDoc);

           
            Assert.IsTrue(biz.Id == accountDoc.Id);
            Assert.IsTrue(biz.Name == accountDoc.Name);
            Assert.IsTrue(biz.Id == accountDoc.Id);

            // createBusinessAccount_maps_properly has been changed to the Repository Pattern with IObjectRepository
            //TODO: Rebuild this test to test AccountsSideBarFactory
        }


        [Test]
        [Ignore]
        public void buildalltentries_returns_entries()
        {
            var entries = _objectRepository.Load<BuildAllEntriesFilter, List<Entry>>(new BuildAllEntriesFilter()
            {
                LedgerId = "1",
                Paging = new PagingInfo { Take = 50, CurrentPage = 1 }
            });

            Assert.IsTrue(entries.Count > 0);

            // buildalltentries_returns_entries has been changed to the Repository Pattern
            //TODO: Rebuild this test to test AccountsSideBarFactory
        }

        [Ignore]
        public void buildaccountssidebar_builds_properly()
        {
            //var model = _builder.BuildAccountSideBar("1");

            //Assert.IsTrue(model.Accounts.Count == 1);
            //Assert.IsTrue(model.CreditCards.Count == 1);
           
            //Assert.IsTrue(model.FormattedAccountsTotalInDollars.Equals("$0.01"));
            //Assert.IsTrue(model.FormattedCreditCardTotalsInDollars.Equals("$0.03"));

            //Build Accounts Sidebar has been changed to the Repository Pattern
            //TODO: Rebuild this test to test AccountsSideBarFactory
           
        }

        
    }
}
