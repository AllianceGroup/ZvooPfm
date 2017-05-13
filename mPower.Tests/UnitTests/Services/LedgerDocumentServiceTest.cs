using System.Collections.Generic;
using NUnit.Framework;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;

namespace mPower.Tests.UnitTests.Services
{
    public class LedgerDocumentServiceTest : BaseServiceTest
    {
        private LedgerDocumentService _service;

        public override void Setup()
        {
            base.Setup();
            _service = _container.GetInstance<LedgerDocumentService>();
        }

        public override IEnumerable<object> Given()
        {
            yield return new LedgerDocument
            {
                Id = _id + "l0",
                Accounts = new List<AccountDocument>
                {
                    new AccountDocument {Id = _id + "l0a0"},
                    new AccountDocument {Id = _id + "l0a1"},
                },
                Users = new List<LedgerUserDocument>
                {
                    new LedgerUserDocument {Id = _id + "l0u0"},
                },
            };

            yield return new LedgerDocument
            {
                Id = _id + "l1",
                Accounts = new List<AccountDocument>
                {
                    new AccountDocument {Id = _id + "l1a0"},
                    new AccountDocument {Id = _id + "l1a1"},
                },
                Users = new List<LedgerUserDocument>
                {
                    new LedgerUserDocument {Id = _id + "l1u0"},
                    new LedgerUserDocument {Id = _id + "l1u1"},
                },
            };
        }

        [Test]
        public void service_corretly_search_ledgers_by_accountId()
        {
            var ledgers = _service.GetByFilter(new LedgerFilter {AccountId = _id + "l1a0"});

            Assert.AreEqual(1, ledgers.Count);
        }

        [Test]
        public void service_corretly_search_ledgers_by_userId()
        {
            var ledgers = _service.GetByFilter(new LedgerFilter {UserId = _id + "l1u0"});

            Assert.AreEqual(1, ledgers.Count);
        }

        [Test]
        public void service_corretly_search_ledgers_by_userId_2()
        {
            var ledgers = _service.GetByUserId(_id + "l1u0");

            Assert.AreEqual(1, ledgers.Count);
        }
    }
}