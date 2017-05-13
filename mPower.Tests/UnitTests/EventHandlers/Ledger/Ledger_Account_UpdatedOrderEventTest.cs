using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Domain.Accounting.Ledger.Events;

namespace mPower.Tests.UnitTests.EventHandlers.Ledger
{
    [TestFixture]
    public class Ledger_Account_UpdatedOrderEventTest: BaseHandlerTest
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
                                            TypeEnum = AccountTypeEnum.Expense,
                                            Order = 1
                                        },
                                    new AccountDocument()
                                        {
                                            Id = "a2",
                                            Name = "Name a2",
                                            TypeEnum = AccountTypeEnum.Asset,
                                            Order = 2
                                        }
                                }
                };
        }

        public override IEnumerable<IEvent> When()
        {
            yield return new Ledger_Account_UpdatedOrderEvent
            {
                LedgerId = _id,
                Orders = new List<AccountOrderData>
                {
                    new AccountOrderData
                        {
                            Id = "a1",
                            Order = 2
                        },
                    new AccountOrderData
                        {
                            Id = "a2",
                            Order = 1
                        }
                }
            };
        }

        public override IEnumerable<object> Expected()
        {
            yield break;
        }

        [Test]
        public void Test()
        {
            Dispatch(AssertOrdering);
        }

        private void AssertOrdering()
        {
            var service = _container.GetInstance<LedgerDocumentService>();
            var ledger = service.GetById(_id);

            Assert.AreEqual(2,ledger.Accounts.Single(x => x.Id == "a1").Order);
            Assert.AreEqual(1,ledger.Accounts.Single(x => x.Id == "a2").Order);

        }
    }
}