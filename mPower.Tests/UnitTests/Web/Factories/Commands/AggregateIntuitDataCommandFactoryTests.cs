using Default.Factories.Commands.Aggregation;
using mPower.Aggregation.Client;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.EventHandlers;
using mPower.Tests.MoqObjects.DocumentServices;
using mPower.Tests.MoqObjects.Intuit;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Linq;

namespace mPower.Tests.UnitTests.Web.Factories.Commands
{
    [TestFixture]
    [Ignore] //test works, but not at prod environment
    public class AggregateIntuitDataCommandFactoryTests : BaseWebTest
    {
        private LedgerDocumentServiceMock _ledgerService;
        private AggregationClientMock _aggregation;
        private AggregateIntuitDataCommandFactory _factory;

        private LedgerDocument FakeLedger { get { return _ledgerService.FakeLedger; } }

        [SetUp]
        public void SetupTest()
        {
            _ledgerService = MockFactory.Create<LedgerDocumentServiceMock>().AddGetById();
            _aggregation = MockFactory.Create<AggregationClientMock>()
                .AddGetInstitutionKeys(FakeLedger.Users.First().Id);
            _container.Configure(config =>
            {
                config.For<LedgerDocumentService>().Use(_ledgerService.Object);
                config.For<IAggregationClient>().Use(_aggregation.Object);
                config.For<IAggregationCallback>().Use<AggregationCallback>();
            });
            var callback = _container.GetInstance<IAggregationCallback>();
            _container.Configure(config => config.AddRegistry(new AggregationClientRegister(callback)));
            _factory = _container.GetInstance<AggregateIntuitDataCommandFactory>();
        }

        [Test]
        public void Intuit_Transactions_UploadCommand_does_not_include_non_intuit_accounts()
        {
            var suitableAccount = new AccountDocument {IntuitAccountId = 11111, Id = Guid.NewGuid().ToString()};
            var notSuitableAccount = new AccountDocument {IntuitAccountId = null, Id = Guid.NewGuid().ToString()};
            FakeLedger.Accounts.AddRange(new[] {suitableAccount, notSuitableAccount});

            var result = _factory.Load(new AggregateUserDto
            {
                LedgerId = FakeLedger.Id, 
                UserId = FakeLedger.Users.First().Id,
            });

            Assert.IsTrue(result.SetStatusCommands.Any(x => x.AccountId == suitableAccount.Id));
            Assert.IsFalse(result.SetStatusCommands.Any(x => x.AccountId == notSuitableAccount.Id));

            Assert.IsTrue(result.IntuitAccountsIds.Any(x => x == suitableAccount.IntuitAccountId));
            Assert.IsFalse(result.IntuitAccountsIds.Any(x => x == notSuitableAccount.IntuitAccountId));
        }

        [Test]
        public void Intuit_Transactions_UploadCommand_does_not_include_accounts_with_wrong_intuit_id()
        {
            var suitableAccount = new AccountDocument {IntuitAccountId = 11111, Id = Guid.NewGuid().ToString()};
            var notSuitableAccount = new AccountDocument {IntuitAccountId = 0, Id = Guid.NewGuid().ToString()};
            FakeLedger.Accounts.AddRange(new[] {suitableAccount, notSuitableAccount});

            var result = _factory.Load(new AggregateUserDto
            {
                LedgerId = FakeLedger.Id,
                UserId = FakeLedger.Users.First().Id,
            });

            Assert.IsTrue(result.SetStatusCommands.Any(x => x.AccountId == suitableAccount.Id));
            Assert.IsFalse(result.SetStatusCommands.Any(x => x.AccountId == notSuitableAccount.Id));

            Assert.IsTrue(result.IntuitAccountsIds.Any(x => x == suitableAccount.IntuitAccountId));
            Assert.IsFalse(result.IntuitAccountsIds.Any(x => x == notSuitableAccount.IntuitAccountId));
        }

        [Test]
        public void Intuit_Transactions_UploadCommand_has_correctly_init_properties()
        {
            var suitableAccount = new AccountDocument {IntuitAccountId = 11111};
            FakeLedger.Accounts.Add(suitableAccount);

            var dto = new AggregateUserDto
            {
                LedgerId = FakeLedger.Id,
                UserId = FakeLedger.Users.First().Id,
            };

            var result = _factory.Load(dto);

            Assert.GreaterOrEqual(result.SetStatusCommands.Count, 1);
            result.SetStatusCommands.ForEach(x => Assert.AreEqual(dto.LedgerId, x.LedgerId));

            Assert.GreaterOrEqual(result.IntuitAccountsIds.Count, 1);
        }

        [Test]
        public void AuthenticateToInstitutionViewModel_is_built_correctly()
        {
            var dto = new AuthenticateDto
            {
                UserId = FakeLedger.Users.First().Id,
                InstitutionId = AggregationClientMock.FakeInstitutionId,
            };

            var model = _factory.Load(dto);

            Assert.AreEqual(dto.InstitutionId, model.ContentServiceId);
            Assert.AreEqual(_aggregation.FakeKeyDocuments.Count(x => x.DisplayFlag), model.Keys.Count);
            for (var i = 0; i < model.Keys.Count - 1; i++)
            {
                Assert.Less(model.Keys[i].DisplayOrder, model.Keys[i + 1].DisplayOrder);
            }
        }
    }
}