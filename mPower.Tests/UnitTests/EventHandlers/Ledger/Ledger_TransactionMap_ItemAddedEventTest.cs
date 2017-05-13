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
    public class Ledger_TransactionMap_ItemAddedEventTest: BaseHandlerTest
    {
        public override IEnumerable<object> Given()
        {
            yield return new LedgerDocument()
                             {
                                 Id = "1",
                                 Name = "Test ledger",
                                 Accounts = new List<AccountDocument>()
                                                {
                                                    new AccountDocument()
                                                        {
                                                            Id = "a11",
                                                            Name = "Name a1",
                                                            TypeEnum = AccountTypeEnum.Expense
                                                        }
                                                }
                             };
            yield return new LedgerDocument()
                             {
                                 Id = "2",
                                 Name = "Test ledger",
                                 Accounts = new List<AccountDocument>()
                                                {
                                                    new AccountDocument()
                                                        {
                                                            Id = "a12",
                                                            Name = "Name a1",
                                                            TypeEnum = AccountTypeEnum.Expense
                                                        },

                                                    new AccountDocument()
                                                        {
                                                            Id = "a22",
                                                            Name = "Name a2",
                                                            TypeEnum = AccountTypeEnum.Expense
                                                        }
                                                },
                                 KeywordMap = new List<KeywordMapDocument>()
                                                  {
                                                      new KeywordMapDocument()
                                                          {
                                                              AccountId = "a12",
                                                              Keyword = "keyword12"
                                                          }
                                                  }
                             };
        }

        public override IEnumerable<IEvent> When()
        {
            yield return new Ledger_TransactionMap_ItemAddedEvent
                             {
                                 AccountId = "a11",
                                 LedgerId = "1",
                                 Keyword = "keyword"
                             };
            yield return new Ledger_TransactionMap_ItemAddedEvent
                             {
                                 AccountId = "a22",
                                 LedgerId = "2",
                                 Keyword = "keyword12"
                             };
        }

        public override IEnumerable<object> Expected()
        {
            yield break;
        }

        [Test]
        public void Test()
        {
            Dispatch(()=>
                         {
                             var service = _container.GetInstance<LedgerDocumentService>();
                             var ledger = service.GetById("1");
                             Assert.NotNull(ledger.KeywordMap);
                             Assert.IsNotEmpty(ledger.KeywordMap);
                             Assert.AreEqual("keyword", ledger.KeywordMap.Single(x => x.AccountId == "a11").Keyword);

                             ledger = service.GetById("2");
                             Assert.AreEqual(1,ledger.KeywordMap.Count);
                             Assert.AreEqual("a22", ledger.KeywordMap.Single(x => x.Keyword == "keyword12").AccountId);
                         });
        }
    }
}