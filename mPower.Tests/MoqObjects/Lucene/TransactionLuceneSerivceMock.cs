using System;
using System.Collections.Generic;
using Moq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Tests.Environment;

namespace mPower.Tests.MoqObjects.Lucene
{
    public class TransactionLuceneSerivceMock : IMock<TransactionLuceneService>
    {
        public TransactionLuceneSerivceMock()
        {
            Create();
        }

        private Mock<TransactionLuceneService> _current;

        public Mock<TransactionLuceneService> Create()
        {
            var settings = new MPowerSettings();
            settings.LuceneIndexesDirectory = String.Empty;

            _current = new Mock<TransactionLuceneService>(settings);

            return _current;
        }

        public TransactionLuceneService Object
        {
            get { return _current.Object; }
        }

        public TransactionLuceneSerivceMock AddSearchByQueryResults()
        {
            _current.Setup(x => x.SearchByQuery(It.IsAny<EntryLuceneFilter>())).Returns(FakeLuceneEntries);

            return this;
        }

        public List<EntryDocument> FakeLuceneEntries
        {
            get
            {
                return new List<EntryDocument>()
                           {
                               new EntryDocument()
                                   {
                                       AccountId = "1",
                                       AccountName = "1",
                                       AccountType = AccountTypeEnum.Asset,
                                       AccountLabel = AccountLabelEnum.FixedAsset,
                                       LedgerId = "1",
                                       Id = "1",
                                       Memo = "memo",
                                       BookedDate = DateTime.UtcNow
                                   },
                               new EntryDocument()
                               {
                                       AccountId = "6",
                                       AccountName = "6",
                                       AccountType = AccountTypeEnum.Expense,
                                       AccountLabel = AccountLabelEnum.Expense,
                                       LedgerId = "1",
                                       Id = "2",
                                       Memo = "memo",
                                       BookedDate = DateTime.UtcNow
                                   }
                           };
            }
        }
    }
}
