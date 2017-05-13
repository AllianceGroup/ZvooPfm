using System;
using System.Collections.Generic;
using Moq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Tests.Environment;
using mPower.Tests.MoqObjects.Mongodb;

namespace mPower.Tests.MoqObjects.DocumentServices
{
    public class EntryDocumentServiceMock : IMock<EntryDocumentService>
    {
        private readonly MockFactory _mockFactory;
        private Mock<EntryDocumentService> _current;

        public EntryDocumentServiceMock(MockFactory mockFactory)
        {
            _mockFactory = mockFactory;
            _current = Create();
        }

        public EntryDocumentService Object
        {
            get { return _current.Object; }
        }

        public Mock<EntryDocumentService> Create()
        {
            var mongoread = _mockFactory.Create<MongoReadMock>();

            return new Mock<EntryDocumentService>(mongoread.Object);
        }

        public EntryDocumentServiceMock AddGetByFilter()
        {
            var entries = new List<EntryDocument>();

            for (int i = 1; i < 20; i++)
            {
                var ii = i.ToString();
                var entry = new EntryDocument()
                {
                    AccountId = ii,
                    BookedDate = DateTime.Now,
                    BookedDateString = DateTime.Now.ToShortDateString(),
                    CreditAmountInCents = i,
                    DebitAmountInCents = 0,
                    Memo = "Memo " + ii,
                    Payee = "Payee " + ii,
                };

                entries.Add(entry);
            }

            _current.Setup(x => x.GetByFilter(It.IsAny<EntryFilter>())).Returns(entries);

            return this;
        }
    }
}
