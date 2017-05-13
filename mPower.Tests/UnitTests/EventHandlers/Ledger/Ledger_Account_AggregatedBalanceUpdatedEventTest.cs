using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;

namespace mPower.Tests.UnitTests.EventHandlers.Ledger
{
    public class Ledger_Account_AggregatedBalanceUpdatedEventTest: BaseHandlerTest
    {
        private const string UserId = "user_123";

        public override IEnumerable<object> Given()
        {
            yield return new LedgerDocument
            {
                Id = _id,
                Name = "Test ledger",
                Accounts = new List<AccountDocument>
                {
                    new AccountDocument
                    {
                        Id = "a1",
                        Name = "Name a1",
                        TypeEnum = AccountTypeEnum.Expense
                    }
                }
            };
            yield return new UserDocument
            {
                Id = UserId,
                ApplicationId = "app_123",
                AffiliateName = "aff_name",
            };
        }

        public override IEnumerable<IEvent> When()
        {
            yield return new Ledger_Account_AggregatedBalanceUpdatedEvent
            {
                AccountId = "a1",
                LedgerId = _id,
                NewBalance = 100,
                UserId = UserId,
            };
        }

        public override IEnumerable<object> Expected()
        {
            yield break;
        }

        [Test]
        public void Test()
        {
            Dispatch(AsserBalanceUpdated);
        }

        private void AsserBalanceUpdated()
        {
            var service = _container.GetInstance<LedgerDocumentService>();
            var ledger = service.GetById(_id);
            var account = ledger.Accounts.Single(x => x.Id == "a1");
            Assert.AreEqual(100, account.AggregatedBalance);
        }
    }
}