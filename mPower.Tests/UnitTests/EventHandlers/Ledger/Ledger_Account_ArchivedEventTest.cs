using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;

namespace mPower.Tests.UnitTests.EventHandlers.Ledger
{
    public class Ledger_Account_ArchivedEventTest: BaseHandlerTest
    {
        public override IEnumerable<object> Given()
        {
            yield return new LedgerDocument()
            {
                Id = _id,
                Name = "Test ledger",
                Accounts = new List<AccountDocument>()
                                {
                                    new AccountDocument()
                                        {
                                            Id = "a1",
                                            Name = "Name a1",
                                            TypeEnum = AccountTypeEnum.Expense
                                        }
                                }
            };
        }

        public override IEnumerable<IEvent> When()
        {
            yield return new Ledger_Account_ArchivedEvent
                             {
                                 AccountId = "a1",
                                 LedgerId = _id,
                                 Reason = "r1"
                             };
        }

        public override IEnumerable<object> Expected()
        {
            yield break;
        }

        [Test]
        public void Test()
        {
            Dispatch(AssertArchived);
        }

        private void AssertArchived()
        {
            var service = _container.GetInstance<LedgerDocumentService>();
            var ledger = service.GetById(_id);
            var account = ledger.Accounts.Single(x => x.Id == "a1");
            Assert.IsTrue(account.Archived);
            Assert.AreEqual("r1", account.ReasonToArchive);
        }
    }
}