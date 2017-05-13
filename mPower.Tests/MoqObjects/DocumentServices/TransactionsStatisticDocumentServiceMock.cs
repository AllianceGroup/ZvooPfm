using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Tests.Environment;
using mPower.Tests.MoqObjects.Mongodb;
using mPower.Tests.MoqObjects.Common;

namespace mPower.Tests.MoqObjects.DocumentServices
{
    public class TransactionsStatisticDocumentServiceMock : IMock<TransactionsStatisticDocumentService>
    {
        private readonly MockFactory _mockFactory;

        private Mock<TransactionsStatisticDocumentService> _current;

        public TransactionsStatisticDocumentServiceMock(MockFactory mockFactory)
        {
            _mockFactory = mockFactory;
            _current = Create();
        }

        public Mock<TransactionsStatisticDocumentService> Create()
        {
            var mongoRead = _mockFactory.Create<MongoReadMock>().Object;
            var iidGenerator = _mockFactory.Create<IIdGeneratorMock>().MockGenerateId("1").Object;
            var mock = new Mock<TransactionsStatisticDocumentService>(mongoRead, iidGenerator);
            mock.SetReturnsDefault(new List<TransactionsStatisticDocument>());
            return mock;
        }

        public TransactionsStatisticDocumentService Object
        {
            get { return _current.Object; }
        }
    }
}
